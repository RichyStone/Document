using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Measure;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.Fwm8612Helper.Model.Enums;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting
{
    public partial class TemperatureViewModel : ObservableObject
    {
        #region Fields

        private FWM8612Context FwmContext => FWM8612Context.GetInstance();

        public bool Connected => FwmContext.Connected;

        /// <summary>
        /// TCM温度1
        /// </summary>
        [ObservableProperty]
        private double? tcm_Temperature1;

        /// <summary>
        /// TCM温度2
        /// </summary>
        [ObservableProperty]
        private double? tcm_Temperature2;

        /// <summary>
        /// 温度监控令牌
        /// </summary>
        private CancellationTokenSource? temperatureTokenSource;

        /// <summary>
        /// 温度下限
        /// </summary>
        private readonly double tempLowLimit = 33 - 0.1;

        /// <summary>
        /// 温度上限
        /// </summary>
        private readonly double tempHighLimit = 33 + 0.1;

        [ObservableProperty]
        private HardWareState state;

        /// <summary>
        /// 温度
        /// </summary>
        [ObservableProperty]
        private double? temprature;

        /// <summary>
        /// 湿度
        /// </summary>
        [ObservableProperty]
        private double? relativeHumidity;

        /// <summary>
        /// 气压
        /// </summary>
        [ObservableProperty]
        private double? airPressure;

        /// <summary>
        /// 硬件状态队列
        /// </summary>
        public Channel<(double, double, double)> HardWareStateChannel { get; } = Channel.CreateBounded<(double, double, double)>(new BoundedChannelOptions(3) { FullMode = BoundedChannelFullMode.DropOldest });

        /// <summary>
        /// 测量监控令牌
        /// </summary>
        private CancellationTokenSource? measureTokenSource;

        #endregion Fields

        #region Function

        [RelayCommand]
        private void Loaded()
        {
            RegisterMessager();
            RegisterCallBack();
        }

        [RelayCommand]
        private void Unloaded()
        {
            UnRegisterMessager();
            UnRegisterCallBack();
        }

        #region Messager

        /// <summary>
        /// 注册Messager
        /// </summary>
        private void RegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState);
            WeakReferenceMessenger.Default.Register<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState, ConnectionStateChangedHandler);

            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState);
            WeakReferenceMessenger.Default.Register<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState, MeasureStateChangedHandler);
        }

        /// <summary>
        /// 解绑Messager
        /// </summary>
        private void UnRegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState);
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState);
        }

        /// <summary>
        /// 连接状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void ConnectionStateChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            if (transData.Value)
            {
                temperatureTokenSource = new CancellationTokenSource();
                new Thread(() => ShowTemprature(temperatureTokenSource.Token)) { IsBackground = true }.Start();
            }
            else
            {
                temperatureTokenSource?.Cancel();
                temperatureTokenSource?.Dispose();
            }
        }

        /// <summary>
        /// 测量状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void MeasureStateChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            if (transData.Value)
            {
                measureTokenSource = new CancellationTokenSource();
                new Thread(() => { HardWareStateVisual(measureTokenSource.Token); }) { IsBackground = true }.Start();
            }
            else
            {
                measureTokenSource?.Cancel();
                measureTokenSource?.Dispose();
            }
        }

        #endregion Messager

        #region CallBack

        /// <summary>
        /// 注册回调
        /// </summary>
        private void RegisterCallBack()
        {
            MeasureContext.CCDDataVisualAction -= HardWareStateHandler;
            MeasureContext.CCDDataVisualAction += HardWareStateHandler;
        }

        /// <summary>
        /// 解绑回调
        /// </summary>
        private void UnRegisterCallBack()
        {
            MeasureContext.CCDDataVisualAction -= HardWareStateHandler;
            while (HardWareStateChannel.Reader.TryRead(out _)) { }
        }

        #endregion CallBack

        /// <summary>
        /// 显示硬件温度
        /// </summary>
        private void ShowTemprature(CancellationToken cancellationToken)
        {
            int errTime = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!Connected)
                    {
                        Thread.Sleep(3000);
                        continue;
                    }

                    var temp1 = FwmContext.GetTCM_Temperature(0);
                    var temp2 = FwmContext.GetTCM_Temperature(1);

                    if (double.TryParse(temp1, out var value1) && double.TryParse(temp2, out var value2))
                    {
                        var normal = value1 > tempLowLimit && value1 < tempHighLimit && value2 > tempLowLimit && value2 < tempHighLimit;
                        var state = normal ? HardWareState.Normal : HardWareState.Error;
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Tcm_Temperature1 = value1;
                            Tcm_Temperature2 = value2;
                            State = state;
                        });
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("获取TCM温度错误", ex);
                    if (errTime++ > 3)
                    {
                        App.Current.Dispatcher.Invoke(() => MessageBoxHelper.ErrorBox("获取TCM温度发生错误！"));
                        break;
                    };
                }
                finally
                {
                    Thread.Sleep(2000);
                }
            }
        }

        /// <summary>
        /// 湿度和压强
        /// </summary>
        /// <param name="RH"></param>
        /// <param name="AP"></param>
        private void HardWareStateHandler(List<int[]> list)
        {
            if (list.Count < 3) return;

            var hardWareParam = (list[2][0], list[2][1], list[2][2]);
            HardWareStateChannel.Writer.TryWrite(hardWareParam);
        }

        /// <summary>
        /// 硬件状态显示
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        private async void HardWareStateVisual(CancellationToken cancellationToken)
        {
            try
            {
                while (await HardWareStateChannel.Reader.WaitToReadAsync(cancellationToken))
                {
                    if (HardWareStateChannel.Reader.TryRead(out var data))
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Temprature = data.Item1 / 100;
                            RelativeHumidity = data.Item2 / 1000;
                            AirPressure = data.Item3 / 1000;
                        });

                    Thread.Sleep(1500);
                }
            }
            catch (OperationCanceledException)
            {
                LogHelper.LogInfo("硬件状态停止更新");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("硬件状态显示错误", ex);
            }
            finally
            {
                while (HardWareStateChannel.Reader.TryRead(out _)) { }
            }
        }

        #endregion Function
    }
}
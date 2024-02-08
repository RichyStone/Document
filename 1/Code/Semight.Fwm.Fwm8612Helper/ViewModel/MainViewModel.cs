using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonModels.Classes.WaveLength;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.CustomUserControl.LogView;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Measure;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Utility;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.ViewManager;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using Semight.Fwm.Common.CommonTools.ExtensionMethods;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Windows.Controls;

namespace Semight.Fwm.Fwm8612Helper.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        #region Fields

        [ObservableProperty]
        private string? waveLength;

        [ObservableProperty]
        private string? power;

        [ObservableProperty]
        private string? waveLengthUnit;

        [ObservableProperty]
        private string? powerUnit;

        /// <summary>
        /// 内部控件
        /// </summary>
        [ObservableProperty]
        private ContentControl? ucLineChart;

        /// <summary>
        /// 结果队列
        /// </summary>
        public Channel<WaveLengthValue> ResultChannel { get; } = Channel.CreateBounded<WaveLengthValue>(new BoundedChannelOptions(10) { FullMode = BoundedChannelFullMode.DropOldest });

        /// <summary>
        /// 测量监控令牌
        /// </summary>
        private CancellationTokenSource? measureTokenSource;

        #endregion Fields

        #region Method

        [RelayCommand]
        private void Loaded(UCLogViewer logViewer)
        {
            RegisterMessager();
            CommonUtility.ReadSoftwareSetting();
            ViewLogger.Register(logViewer);

            MeasureContext.WaveLengthVisualAction -= VisualActionHandler;
            MeasureContext.WaveLengthVisualAction += VisualActionHandler;
            UcLineChart = ViewManager.GetUserControl(ControlType.CavityChart);
        }

        [RelayCommand]
        private void Unloaded()
        {
            CommonUtility.UnRegisterUSBWatcher();
            Process.GetCurrentProcess().Kill();
        }

        #region ResultVisual

        /// <summary>
        /// 数据回调处理
        /// </summary>
        /// <param name="power"></param>
        /// <param name="waveLength"></param>
        private void VisualActionHandler(WaveLengthValue waveLengthValue)
        {
            ResultChannel.Writer.TryWrite(waveLengthValue);
        }

        /// <summary>
        /// 显示结果
        /// </summary>
        private async void ShowWaveLengthResult(CancellationToken cancellationToken)
        {
            try
            {
                while (await ResultChannel.Reader.WaitToReadAsync(cancellationToken))
                {
                    if (ResultChannel.Reader.TryRead(out var data))
                        App.Current.Dispatcher.Invoke(() => RefreshResult(data));

                    await 100;
                }
            }
            catch (OperationCanceledException)
            {
                LogHelper.LogInfo("波长结果数据停止更新");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("波长结果数据显示发生错误", ex);
            }
            finally
            {
                while (ResultChannel.Reader.TryRead(out _)) { }
            }
        }

        /// <summary>
        /// 显示当前结果
        /// </summary>
        /// <param name="power"></param>
        /// <param name="waveLength"></param>
        private void RefreshResult(WaveLengthValue data)
        {
            if (data.OverExposed)
                WaveLength = "OverExposed";
            else
            {
                var waveLength = CommonUtility.GetWavelengthByUnit(GlobalConfig.WaveLengthUnit, data.Final);
                WaveLength = waveLength.ToString("f5");
            }

            var power = CommonUtility.GetPowerByUnit(GlobalConfig.PowerUnit, data.Power);
            Power = power.ToString("f4");
        }

        #endregion ResultVisual

        #region Messager

        /// <summary>
        /// 注册Messager
        /// </summary>
        private void RegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState);
            WeakReferenceMessenger.Default.Register<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState, MeasureStateChangedHandler);

            WeakReferenceMessenger.Default.Unregister<MessagerTransData<WaveLengthUnit>, string>(this, MessagerProtocal.WavelengthUnitChanged);
            WeakReferenceMessenger.Default.Register<MessagerTransData<WaveLengthUnit>, string>(this, MessagerProtocal.WavelengthUnitChanged, WavelengthUnitChangedHandler);

            WeakReferenceMessenger.Default.Unregister<MessagerTransData<PowerUnit>, string>(this, MessagerProtocal.PowerUnitChanged);
            WeakReferenceMessenger.Default.Register<MessagerTransData<PowerUnit>, string>(this, MessagerProtocal.PowerUnitChanged, PowerUnitChangedHandler);
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
                new Thread(() => { ShowWaveLengthResult(measureTokenSource.Token); }) { IsBackground = true }.Start();
            }
            else
            {
                measureTokenSource?.Cancel();
                measureTokenSource?.Dispose();
            }
        }

        /// <summary>
        /// 波长单位改变事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messagerTransData"></param>
        private void WavelengthUnitChangedHandler(object sender, MessagerTransData<WaveLengthUnit> messagerTransData)
        {
            if (messagerTransData.Value == Fwm.HardWare.FWM8612InteractionLib.Models.Enums.WaveLengthUnit.Cm_1)
                WaveLengthUnit = "/Cm";
            else
                WaveLengthUnit = messagerTransData.Value.ToString();
        }

        /// <summary>
        /// 功率单位改变事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messagerTransData"></param>
        private void PowerUnitChangedHandler(object sender, MessagerTransData<PowerUnit> messagerTransData)
        {
            PowerUnit = messagerTransData.Value.ToString();
        }

        #endregion Messager

        #endregion Method
    }
}
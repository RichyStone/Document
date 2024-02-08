using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveCharts;
using LiveCharts.Wpf;
using NationalInstruments.Restricted;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Measure;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Windows.Input;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.Chart
{
    public partial class CavityChartViewModel : ObservableObject
    {
        /// <summary>
        /// 薄腔数据
        /// </summary>
        public ChartValues<int> ThinCavity { get; set; } = new();

        /// <summary>
        /// 厚腔数据
        /// </summary>
        public ChartValues<int> ThickCavity { get; set; } = new();

        /// <summary>
        /// CCD数据集合
        /// </summary>
        public Channel<(int[], int[])> CavityChannel { get; } = Channel.CreateBounded<(int[], int[])>(new BoundedChannelOptions(3) { FullMode = BoundedChannelFullMode.DropOldest });

        /// <summary>
        /// 测量监控令牌
        /// </summary>
        private CancellationTokenSource? measureTokenSource;

        [ObservableProperty]
        private double xMin;

        [ObservableProperty]
        private double xMax = 512;

        [ObservableProperty]
        private double yMin;

        [ObservableProperty]
        private double yMax = 65535;

        #region Event

        [RelayCommand]
        private void Loaded()
        {
            RegisterCallBack();
            RegisterMessager();
        }

        [RelayCommand]
        private void Unloaded()
        {
            UnRegisterCallBack();
            UnRegisterMessager();
            ClearChart();
        }

        [RelayCommand]
        private void InitialChartSize(MouseButtonEventArgs e)
        {
            if (e.Source is not CartesianChart chart)
                return;

            chart.AxisX.SafeForEach(x => { x.MinValue = 0; x.MaxValue = 512; });
            chart.AxisY.SafeForEach(x => { x.MinValue = 0; x.MaxValue = 65535; });
        }

        #endregion Event

        #region Messager

        /// <summary>
        /// 注册Messager
        /// </summary>
        private void RegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState);
            WeakReferenceMessenger.Default.Register<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState, MeasureStateChangedHandler);
        }

        /// <summary>
        /// 解绑Messager
        /// </summary>
        private void UnRegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState);
        }

        /// <summary>
        /// 测量状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void MeasureStateChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            TurnSwitchVisualThread(transData.Value);
        }

        #endregion Messager

        #region CallBack

        /// <summary>
        /// 注册回调
        /// </summary>
        private void RegisterCallBack()
        {
            MeasureContext.CCDDataVisualAction -= CavityDataHandler;
            MeasureContext.CCDDataVisualAction += CavityDataHandler;
        }

        /// <summary>
        /// 解绑回调
        /// </summary>
        private void UnRegisterCallBack()
        {
            MeasureContext.CCDDataVisualAction -= CavityDataHandler;
        }

        /// <summary>
        /// 回调Handler
        /// </summary>
        /// <param name="narrow"></param>
        /// <param name="thick"></param>
        private void CavityDataHandler(List<int[]> data)
        {
            if (data.Count < 3) return;
            CavityChannel.Writer.TryWrite((data[0], data[1]));
        }

        #endregion CallBack

        #region Function

        /// <summary>
        /// 开关数据显示线程
        /// </summary>
        /// <param name="onOff"></param>
        private void TurnSwitchVisualThread(bool onOff)
        {
            if (onOff)
            {
                measureTokenSource = new CancellationTokenSource();
                new Thread(() => { ShowCavityData(measureTokenSource.Token); }) { IsBackground = true }.Start();
            }
            else
            {
                measureTokenSource?.Cancel();
                measureTokenSource?.Dispose();
            }
        }

        /// <summary>
        /// 显示CCD数据
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async void ShowCavityData(CancellationToken cancellationToken)
        {
            try
            {
                while (await CavityChannel.Reader.WaitToReadAsync(cancellationToken))
                {
                    if (CavityChannel.Reader.TryRead(out var data))
                        App.Current.Dispatcher.Invoke(() => RefreshChart(data.Item1, data.Item2));

                    Thread.Sleep(100);
                }
            }
            catch (OperationCanceledException)
            {
                LogHelper.LogInfo("CCD数据显示停止更新");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("CCD数据显示发生错误", ex);
            }
            finally
            {
                while (CavityChannel.Reader.TryRead(out _)) { }
            }
        }

        /// <summary>
        /// 显示数据
        /// </summary>
        /// <param name="dataThinc"></param>
        /// <param name="dataThick"></param>
        private void RefreshChart(int[] dataThinc, int[] dataThick)
        {
            ClearChart();
            ThinCavity.AddRange(dataThinc);
            ThickCavity.AddRange(dataThick);
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        private void ClearChart()
        {
            ThinCavity.Clear();
            ThickCavity.Clear();
        }

        #endregion Function
    }
}
//#define DiffLog

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonModels.Classes.WaveLength;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.CustomUserControl.WaveLengthChart.View;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Measure;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Utility;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.Chart
{
    public partial class WaveLengthChartViewModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// 测量监控令牌
        /// </summary>
        private CancellationTokenSource? measureTokenSource;

        /// <summary>
        /// 结果集合
        /// </summary>
        public Channel<WaveLengthValue> MeasureResults { get; } = Channel.CreateBounded<WaveLengthValue>(new BoundedChannelOptions(100_000) { FullMode = BoundedChannelFullMode.DropNewest });

        /// <summary>
        /// 显示数据索引
        /// </summary>
        private int visualIndex = 0;

        [ObservableProperty]
        private bool overExposed;

        [ObservableProperty]
        private int viewRange;

        #region Chart

        [ObservableProperty]
        private UcScottPlot? plot = new();

        #endregion Chart

        #region Grid

        /// <summary>
        /// 已测数据量
        /// </summary>
        [ObservableProperty]
        private int count;

        [ObservableProperty]
        private WaveLengthValue? wavelength;

        #endregion Grid

        #endregion Fields

        [RelayCommand]
        private void Loaded()
        {
            ViewRange = GlobalConfig.ChartSetting.ViewRange;

            Plot?.InitialChartView();
            Plot?.ChangeWavelengthUnit($"{GlobalConfig.WaveLengthUnit}");
            Plot?.ChangePowerUnit($"{GlobalConfig.PowerUnit}");

            Plot?.SetLineSeriesVisible(GlobalConfig.ChartSetting.WaveLengthVisible, 0);
            Plot?.SetLineSeriesVisible(GlobalConfig.ChartSetting.PowerVisible, 1);
            Plot?.SetViewRange(ViewRange);

            RegisterCallBack();
            RegisterMessager();
            TurnSwitchVisualThread(true);
        }

        [RelayCommand]
        private void Unloaded()
        {
            UnRegisterCallBack();
            UnRegisterMessager();
            TurnSwitchVisualThread(false);
        }

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
        /// 解绑Messager
        /// </summary>
        private void UnRegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.CalculateTypeChanged);
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.MeasureState);
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<WaveLengthUnit>, string>(this, MessagerProtocal.WavelengthUnitChanged);
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<PowerUnit>, string>(this, MessagerProtocal.PowerUnitChanged);
        }

        /// <summary>
        /// 测量状态改变回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void MeasureStateChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            if (transData.Value)
            {
                Plot?.InitialData();
                while (MeasureResults.Reader.TryRead(out _)) { }
            }
        }

        /// <summary>
        /// 波长单位改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messagerTransData"></param>
        private void WavelengthUnitChangedHandler(object sender, MessagerTransData<WaveLengthUnit> messagerTransData)
        {
            Plot?.ChangeWavelengthUnit(messagerTransData.Value.ToString());
            Plot?.InitialData();
        }

        /// <summary>
        /// 功率单位改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="messagerTransData"></param>
        private void PowerUnitChangedHandler(object sender, MessagerTransData<PowerUnit> messagerTransData)
        {
            Plot?.ChangePowerUnit(messagerTransData.Value.ToString());
            Plot?.InitialData();
        }

        #endregion Messager

        #region CallBack

        /// <summary>
        /// 注册回调
        /// </summary>
        private void RegisterCallBack()
        {
            MeasureContext.WaveLengthVisualAction -= VisualActionHandler;
            MeasureContext.WaveLengthVisualAction += VisualActionHandler;
        }

        /// <summary>
        /// 解绑回调
        /// </summary>
        private void UnRegisterCallBack()
        {
            MeasureContext.WaveLengthVisualAction -= VisualActionHandler;
        }

        /// <summary>
        /// 显示回调处理
        /// </summary>
        /// <param name="power"></param>
        /// <param name="wavelength"></param>
        private void VisualActionHandler(WaveLengthValue wavelength)
        {
            MeasureResults.Writer.TryWrite(wavelength);
        }

        #endregion CallBack

        #region Event

        [RelayCommand]
        private void XAxisChanged()
        {
            if (GlobalConfig.ChartSetting.PointXAxis)
                Plot?.ChangeXAxis(false);
            else if (GlobalConfig.ChartSetting.TimeXAxis)
                Plot?.ChangeXAxis(true);
        }

        [RelayCommand]
        private void WaveLengthVisibleChanged()
        {
            Plot?.SetLineSeriesVisible(GlobalConfig.ChartSetting.WaveLengthVisible, 0);
        }

        [RelayCommand]
        private void PowerVisibleChanged()
        {
            Plot?.SetLineSeriesVisible(GlobalConfig.ChartSetting.PowerVisible, 1);
        }

        /// <summary>
        /// 重置事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [RelayCommand]
        private void ResetUpdate()
        {
            Plot?.InitialData();
            while (MeasureResults.Reader.TryRead(out _)) { }
            RegisterCallBack();
        }

        /// <summary>
        /// 停止刷新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [RelayCommand]
        private void StopUpdate()
        {
            UnRegisterCallBack();
        }

        /// <summary>
        /// 导出数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [RelayCommand]
        private void ExportData()
        {
            var savePath = GlobalConfig.ChartSetting.DataSavePath;
            var dir = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(savePath) && !string.IsNullOrEmpty(dir))
            {
                var extension = Path.GetExtension(savePath);
                var fileName = Path.GetFileNameWithoutExtension(savePath);
                fileName += $"_{DateTime.Now:yyyyMMdd_Hmmss}.{extension}";
                Plot?.ExportData(Path.Combine(dir, fileName));
            }
        }

        /// <summary>
        /// 更改保存路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [RelayCommand]
        private void ChangeSavePath()
        {
            try
            {
                var str = CommonUtility.GetSaveFileRoute("Excel(*.csv)|*.xlsx", "TempData.xlsx");
                if (string.IsNullOrWhiteSpace(str)) return;

                var path = System.IO.Path.GetDirectoryName(str);

                if (string.IsNullOrEmpty(path))
                {
                    MessageBox.Show($"保存路径不存在：{str}");
                    return;
                }

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                GlobalConfig.ChartSetting.DataSavePath = str;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ErrorBox($"定义保存路径错误：{ex.Message}");
            }
        }

        partial void OnViewRangeChanged(int value)
        {
            GlobalConfig.ChartSetting.ViewRange = value;
            Plot?.SetViewRange(value);
        }

        #endregion Event

        #region Function

        /// <summary>
        /// 开关数据显示线程
        /// </summary>
        /// <param name="onOff"></param>
        private void TurnSwitchVisualThread(bool onOff)
        {
            if (measureTokenSource != null && !measureTokenSource.IsCancellationRequested)
            {
                measureTokenSource?.Cancel();
                measureTokenSource?.Dispose();
            }

            if (onOff)
            {
                measureTokenSource = new CancellationTokenSource();
                new Thread(() => { RefreshVisual(measureTokenSource.Token); }) { IsBackground = true }.Start();
            }
        }

        /// <summary>
        /// 刷新波长结果显示
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async void RefreshVisual(CancellationToken cancellationToken)
        {
            try
            {
                while (await MeasureResults.Reader.WaitToReadAsync(cancellationToken))
                {
                    if (Plot == null)
                    {
                        await Task.Delay(300, cancellationToken);
                        continue;
                    }

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        bool refreshFlag = false;
                        if (Plot != null)
                        {
                            refreshFlag = Plot.CurDataIndex >= visualIndex + GlobalConfig.ChartSetting.RefreshInterval;

                            if (refreshFlag || Plot.CurDataIndex < visualIndex)
                                visualIndex = Plot.CurDataIndex;

                            if (GlobalConfig.SoftwareSetting.SaveDataToLocalAuto && Plot.CurDataIndex > 0 && Plot.CurDataIndex % 100_000 == 0)
                                ExportData();
                        }

                        if (MeasureResults.Reader.TryRead(out var data))
                        {
                            if (!data.OverExposed)
                            {
                                var power = CommonUtility.GetPowerByUnit(GlobalConfig.PowerUnit, data.Power);
                                var waveLength = CommonUtility.GetWavelengthByUnit(GlobalConfig.WaveLengthUnit, data.Final);

                                if (refreshFlag)
                                    RefreshGrid(data);

                                Plot?.NewDataHandler(Math.Round(waveLength, 7), Math.Round(power, 4), refreshFlag);

                                OverExposed = false;
                            }
                            else
                                OverExposedHandler();
                        }
                    });

                    await Task.Delay(1, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                LogHelper.LogInfo("波长折线图停止刷新");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("波长折线图刷新发生错误", ex);
                MessageBoxHelper.ErrorBox("波长折线图刷新发生错误！");
            }
            finally
            {
                while (MeasureResults.Reader.TryRead(out _)) { }
            }
        }

        /// <summary>
        /// 表格数据刷新
        /// </summary>
        /// <param name="wavelength"></param>
        private void RefreshGrid(WaveLengthValue wavelength)
        {
            Wavelength = wavelength;

            if (Plot != null)
                Count = visualIndex;
        }

        private void OverExposedHandler()
        {
            Plot?.InitialData();
            Count = 0;
            Wavelength = null;
            OverExposed = true;
            while (MeasureResults.Reader.TryRead(out _)) { }
        }

        #endregion Function
    }
}
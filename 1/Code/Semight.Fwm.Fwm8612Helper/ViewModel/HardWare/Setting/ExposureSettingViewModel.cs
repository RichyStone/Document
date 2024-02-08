using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Measure;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting
{
    public partial class ExposureSettingViewModel : ObservableValidator
    {
        #region Fields

        private FWM8612Context FwmContext => FWM8612Context.GetInstance();

        public bool Connected => FwmContext.Connected;

        /// <summary>
        /// 曝光时间
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ManualSetCommand))]
        [NotifyDataErrorInfo]
        [Range(1, 9999, ErrorMessage = "Out of Range，Must Between 1 and 9999")]
        private int fPGA_IntergrationTime;

        /// <summary>
        /// 自适应曝光
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ManualSetCommand))]
        [NotifyCanExecuteChangedFor(nameof(RunAutoExpoOnceCommand))]
        private bool exportAdpativeAuto;

        [ObservableProperty]
        private string? gainStatus;

        #endregion Fields

        #region Messager

        /// <summary>
        /// 注册Messager
        /// </summary>
        private void RegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState);
            WeakReferenceMessenger.Default.Register<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState, ConnectionChangedHandler);

            WeakReferenceMessenger.Default.Unregister<MessagerTransData, string>(this, MessagerProtocal.RefreshHardWareSettings);
            WeakReferenceMessenger.Default.Register<MessagerTransData, string>(this, MessagerProtocal.RefreshHardWareSettings, RefreshHandler);

            WeakReferenceMessenger.Default.Unregister<MessagerTransData<GainState>, string>(this, MessagerProtocal.GainStateChanged);
            WeakReferenceMessenger.Default.Register<MessagerTransData<GainState>, string>(this, MessagerProtocal.GainStateChanged, GainStateChangedHandler);
        }

        /// <summary>
        /// 连接状态改变回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void ConnectionChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            if (transData.Value)
                InitialExpoSettings();

            ManualSetCommand.NotifyCanExecuteChanged();
            ExpoTimeAdaptiveChangedCommand.NotifyCanExecuteChanged();
            RunAutoExpoOnceCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 刷新设定回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void RefreshHandler(object sender, MessagerTransData transData)
        {
            if (Connected)
                InitialExpoSettings();
        }

        /// <summary>
        /// 增益状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void GainStateChangedHandler(object sender, MessagerTransData<GainState> transData)
        {
            GainStatus = FwmContext.GetGainState(1).ToString();
        }

        #endregion Messager

        #region Function

        [RelayCommand]
        private void Loaded()
        {
            RegisterMessager();
        }

        [RelayCommand]
        private void Unloaded()
        {
        }

        /// <summary>
        /// 获取曝光时间设置
        /// </summary>
        private void InitialExpoSettings()
        {
            try
            {
                FPGA_IntergrationTime = FwmContext.GetFPGA_IntergrationTime();
                ExportAdpativeAuto = FwmContext.GetExpoTimeAdaptive();
                GainStatus = FwmContext.GetGainState(1).ToString();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("获取设定发生错误", ex);
                MessageBoxHelper.ErrorBox("获取设定发生错误！");
            }
        }

        /// <summary>
        /// 手动设置可执行
        /// </summary>
        public bool ManualSetting => !ExportAdpativeAuto && FPGA_IntergrationTime > 0 && Connected;

        /// <summary>
        /// 手动设置
        /// </summary>
        [RelayCommand(CanExecute = nameof(ManualSetting))]
        private void ManualSet()
        {
            try
            {
                FwmContext.FPGA_SetIntergrationTime(FPGA_IntergrationTime);
                FPGA_IntergrationTime = FwmContext.GetFPGA_IntergrationTime();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 自适应曝光状态改变
        /// </summary>
        [RelayCommand(CanExecute = nameof(Connected))]
        private void ExpoTimeAdaptiveChanged()
        {
            try
            {
                FwmContext.FPGA_SetExpoTimeAdaptive(ExportAdpativeAuto);
                ExportAdpativeAuto = FwmContext.GetExpoTimeAdaptive();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 一次曝光可执行
        /// </summary>
        public bool RunAutoOnce => !ExportAdpativeAuto && Connected;

        /// <summary>
        /// 运行一次曝光
        /// </summary>
        [RelayCommand(CanExecute = nameof(RunAutoOnce))]
        private async Task RunAutoExpoOnce()
        {
            try
            {
                MeasureContext.PauseMeasure();
                FwmContext.FPGA_SetExpoTimeAuto();
                FPGA_IntergrationTime = FwmContext.GetFPGA_IntergrationTime();
                GainStatus = FwmContext.GetGainState(1).ToString();
                await Task.Delay(500);
                MeasureContext.RestartMeasure();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("运行一次曝光发生错误", ex);
                MessageBoxHelper.ErrorBox("运行一次曝光发生错误！");
            }
        }

        #endregion Function
    }
}
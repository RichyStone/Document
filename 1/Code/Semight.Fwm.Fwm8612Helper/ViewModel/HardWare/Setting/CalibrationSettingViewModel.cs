using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting
{
    public partial class CalibrationSettingViewModel : ObservableObject
    {
        private FWM8612Context FwmContext => FWM8612Context.GetInstance();

        public bool Connected => FwmContext.Connected;

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
        /// 注册Messager
        /// </summary>
        private void RegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState);
            WeakReferenceMessenger.Default.Register<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState, ConnectionChangedHandler);

            WeakReferenceMessenger.Default.Unregister<MessagerTransData, string>(this, MessagerProtocal.RefreshHardWareSettings);
            WeakReferenceMessenger.Default.Register<MessagerTransData, string>(this, MessagerProtocal.RefreshHardWareSettings, RefreshHandler);
        }

        /// <summary>
        /// 连接状态改变回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void ConnectionChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            if (transData.Value)
                IntialCalibrationSetting();

            ReadLastCalibrationTimeCommand.NotifyCanExecuteChanged();
            ReadAutoCalibrationCycleCommand.NotifyCanExecuteChanged();
            WriteAutoCalibrationCycleCommand.NotifyCanExecuteChanged();
            AutoOnOffChangedCommand.NotifyCanExecuteChanged();
            ManualCalibrationOnceCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 刷新设定回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void RefreshHandler(object sender, MessagerTransData transData)
        {
            if (Connected)
                IntialCalibrationSetting();
        }

        /// <summary>
        /// 上次校准事件
        /// </summary>
        [ObservableProperty]
        private string? lastCalibrationTime;

        /// <summary>
        /// 自动校准周期
        /// </summary>
        [ObservableProperty]
        private int autoCalibrationCycle;

        /// <summary>
        /// 是否开启自动校准
        /// </summary>
        [ObservableProperty]
        private bool autoOn;

        /// <summary>
        /// 查询上次校准事件
        /// </summary>
        [RelayCommand(CanExecute = nameof(Connected))]
        private void ReadLastCalibrationTime()
        {
            try
            {
                LastCalibrationTime = FwmContext.QueryLastCalibrationTime();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("获取设定发生错误", ex);
            }
        }

        /// <summary>
        /// 读取自动校准周期
        /// </summary>
        [RelayCommand(CanExecute = nameof(Connected))]
        private void ReadAutoCalibrationCycle()
        {
            try
            {
                AutoCalibrationCycle = FwmContext.QueryAutoCalibrationCycle();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("获取设定发生错误", ex);
            }
        }

        public bool CanSetCalibrationCycle => Connected && AutoCalibrationCycle > 0;

        /// <summary>
        /// 设置自动校准周期
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSetCalibrationCycle))]
        private void WriteAutoCalibrationCycle()
        {
            try
            {
                FwmContext.SetAutoCalibrationCycle(AutoCalibrationCycle);

                AutoCalibrationCycle = FwmContext.QueryAutoCalibrationCycle();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 自动校准
        /// </summary>
        [RelayCommand(CanExecute = nameof(Connected))]
        private void AutoOnOffChanged()
        {
            try
            {
                if (AutoOn)
                    FwmContext.SetAutoCalibration(AutoCalibration.ON);
                else
                    FwmContext.SetAutoCalibration(AutoCalibration.OFF);

                AutoOn = FwmContext.QueryAutoCalibration();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 手动校准一次
        /// </summary>
        [RelayCommand(CanExecute = nameof(Connected))]
        private void ManualCalibrationOnce()
        {
            try
            {
                FwmContext.SetAutoCalibration(AutoCalibration.ONCE);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("手动校准发生错误", ex);
                MessageBoxHelper.ErrorBox("手动校准发生错误！");
            }
        }

        /// <summary>
        /// 初始化校准设定
        /// </summary>
        private void IntialCalibrationSetting()
        {
            try
            {
                LastCalibrationTime = FwmContext.QueryLastCalibrationTime();
                AutoCalibrationCycle = FwmContext.QueryAutoCalibrationCycle();

                AutoOn = FwmContext.QueryAutoCalibration();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("获取设定发生错误", ex);
                MessageBoxHelper.ErrorBox("获取设定发生错误！");
            }
        }
    }
}
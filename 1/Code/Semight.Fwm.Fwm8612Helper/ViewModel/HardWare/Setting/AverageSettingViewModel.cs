using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;
using System.ComponentModel.DataAnnotations;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting
{
    public partial class AverageSettingViewModel : ObservableValidator
    {
        private FWM8612Context FwmContext => FWM8612Context.GetInstance();

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
                InitialAverage();

            SetAverageSettingCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 刷新设定回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void RefreshHandler(object sender, MessagerTransData transData)
        {
            if (FwmContext.Connected)
                InitialAverage();
        }

        [ObservableProperty]
        [Range(1, 127, ErrorMessage = "Out of Range，Must Between 1 and 127")]
        [NotifyDataErrorInfo]
        [NotifyCanExecuteChangedFor(nameof(SetAverageSettingCommand))]
        private int averageCount;

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitialAverage()
        {
            try
            {
                AverageCount = FwmContext.GetAverageCount();
                GlobalConfig.Average = AverageCount;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("获取设定发生错误", ex);
            }

            OnPropertyChanged(nameof(AverageCount));
        }

        public bool CanSetAverage => AverageCount > 0;

        /// <summary>
        /// 设置平均数
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSetAverage))]
        private void SetAverageSetting()
        {
            try
            {
                if (FwmContext.Connected && GlobalConfig.SoftwareSetting.LowerComputerCalculate)
                    FwmContext.SetAverageCount(AverageCount);
                GlobalConfig.Average = AverageCount;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }
    }
}
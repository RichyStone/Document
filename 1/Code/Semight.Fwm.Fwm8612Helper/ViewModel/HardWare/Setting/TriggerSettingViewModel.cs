using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Measure;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting
{
    public partial class TriggerSettingViewModel : ObservableValidator
    {
        private FWM8612Context FwmContext => FWM8612Context.GetInstance();

        public bool Connected => FWM8612Context.GetInstance().Connected;

        public bool InitialFlag { get; private set; }

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
                InitialTriggerSetting();
            else
                InitialFlag = false;

            SetFrequencyCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 刷新设定回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void RefreshHandler(object sender, MessagerTransData transData)
        {
            if (Connected)
            {
                InitialFlag = false;
                InitialTriggerSetting();
            }
        }

        /// <summary>
        /// 软触发
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SetFrequencyCommand))]
        private bool softWareTrigger;

        async partial void OnSoftWareTriggerChanged(bool value)
        {
            if (!value || !Connected || !InitialFlag) return;
            try
            {
                FwmContext.OpenCCD(false);
                await Task.Delay(300);

                FwmContext.SetTriggerMode(FWMTriggerMode.SoftWare);
                TrigFrequency = FwmContext.GetTrigFrequency();
                GlobalConfig.TriggerType = FWMTriggerMode.SoftWare;
                FwmContext.OpenCCD(true);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 外触发
        /// </summary>
        [ObservableProperty]
        private bool externalTrigger;

        async partial void OnExternalTriggerChanged(bool value)
        {
            if (!value || !Connected || !InitialFlag) return;

            try
            {
                FwmContext.OpenCCD(false);
                await Task.Delay(300);

                FwmContext.SetTriggerMode(FWMTriggerMode.External);
                GlobalConfig.TriggerType = FWMTriggerMode.External;
                FwmContext.OpenCCD(true);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 单次触发
        /// </summary>
        [ObservableProperty]
        private bool singletonTrigger;

        async partial void OnSingletonTriggerChanged(bool value)
        {
            if (!value || !Connected || !InitialFlag) return;

            try
            {
                MeasureContext.PauseMeasure();
                FwmContext.OpenCCD(false);

                await Task.Delay(300);
                FwmContext.SetTriggerMode(FWMTriggerMode.Singleton);
                GlobalConfig.TriggerType = FWMTriggerMode.Singleton;
                FwmContext.OpenCCD(true);
                MeasureContext.RestartMeasure();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 触发频率
        /// </summary>
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(1, 1000, ErrorMessage = "Out of Range，Must Between 1 and 1000")]
        private int trigFrequency;

        /// <summary>
        /// 设置触发频率可执行
        /// </summary>
        public bool CanSetFrequency => Connected && SoftWareTrigger;

        /// <summary>
        /// 设置触发频率
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSetFrequency))]
        private void SetFrequency()
        {
            try
            {
                FwmContext.SetTrigFrequency(TrigFrequency);
                TrigFrequency = FwmContext.GetTrigFrequency();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 初始化触发设定
        /// </summary>
        private void InitialTriggerSetting()
        {
            try
            {
                var mode = FwmContext.GetTrigMode();
                switch (mode)
                {
                    case FWMTriggerMode.Singleton: SingletonTrigger = true; break;
                    case FWMTriggerMode.SoftWare: SoftWareTrigger = true; break;
                    case FWMTriggerMode.External: ExternalTrigger = true; break;
                }

                TrigFrequency = FwmContext.GetTrigFrequency();
                InitialFlag = true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("读取设定发生错误", ex);
            }
        }
    }
}
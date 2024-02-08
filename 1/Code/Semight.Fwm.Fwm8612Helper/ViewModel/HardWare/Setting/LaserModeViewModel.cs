using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting
{
    public partial class LaserModeViewModel : ObservableObject
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
                InitialWaveBand();
            else
                InitialFlag = false;
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
                InitialWaveBand();
            }
        }

        [ObservableProperty]
        private bool narrowBandChecked;

        partial void OnNarrowBandCheckedChanged(bool value)
        {
            if (!Connected || !value || !InitialFlag) return;

            try
            {
                FwmContext.SetWaveBand(CavityType.Narrow);
                GlobalConfig.CavityType = CavityType.Narrow;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        [ObservableProperty]
        private bool broadBandChecked;

        partial void OnBroadBandCheckedChanged(bool value)
        {
            if (!Connected || !value || !InitialFlag) return;

            try
            {
                FwmContext.SetWaveBand(CavityType.Broad);
                GlobalConfig.CavityType = CavityType.Broad;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitialWaveBand()
        {
            try
            {
                var band = FwmContext.GetWaveBand();
                if (band == CavityType.Narrow)
                    NarrowBandChecked = true;
                else
                    BroadBandChecked = true;

                InitialFlag = true;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("获取设定发生错误", ex);
                MessageBoxHelper.ErrorBox("获取设定发生错误！");
            }
        }
    }
}
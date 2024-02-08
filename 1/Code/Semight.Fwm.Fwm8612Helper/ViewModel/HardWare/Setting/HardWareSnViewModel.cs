using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonModels.Classes.HardWare;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting
{
    public partial class HardWareSnViewModel : ObservableObject
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
        }

        /// <summary>
        /// 连接状态改变回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void ConnectionChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            if (transData.Value)
                InitialIDN();
        }

        [ObservableProperty]
        private IDNInfo? iDNInfo;

        /// <summary>
        /// 读取IDN
        /// </summary>
        private void InitialIDN()
        {
            try
            {
                IDNInfo = FwmContext.QueryIDN();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("获取仪表信息发生错误", ex);
                MessageBoxHelper.ErrorBox("获取仪表信息发生错误！");
            }
        }
    }
}
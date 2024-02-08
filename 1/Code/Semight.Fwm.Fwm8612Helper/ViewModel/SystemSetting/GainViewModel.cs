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

namespace Semight.Fwm.Fwm8612Helper.ViewModel.SystemSetting
{
    public partial class GainViewModel : ObservableObject
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
        }

        /// <summary>
        /// 连接状态改变回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void ConnectionChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            GainHighCommand.NotifyCanExecuteChanged();
            GainLowCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 开启高增益
        /// </summary>
        [RelayCommand(CanExecute = nameof(Connected))]
        private void GainHigh()
        {
            try
            {
                FwmContext.SetGainState(1, GainState.High);
                WeakReferenceMessenger.Default.Send(new MessagerTransData<GainState> { Value = GainState.High }, MessagerProtocal.GainStateChanged);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }

        /// <summary>
        /// 开启低增益
        /// </summary>
        [RelayCommand(CanExecute = nameof(Connected))]
        private void GainLow()
        {
            try
            {
                FwmContext.SetGainState(1, GainState.Low);
                WeakReferenceMessenger.Default.Send(new MessagerTransData<GainState> { Value = GainState.Low }, MessagerProtocal.GainStateChanged);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }
    }
}
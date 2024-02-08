using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare
{
    public partial class HardwareSettingViewModel : ObservableValidator
    {
        public bool Connected => FWM8612Context.GetInstance().Connected;

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
            RefreshCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 刷新设定参数
        /// </summary>
        [RelayCommand(CanExecute = nameof(Connected))]
        private void Refresh()
        {
            WeakReferenceMessenger.Default.Send(new MessagerTransData(), MessagerProtocal.RefreshHardWareSettings);
        }
    }
}
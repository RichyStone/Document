using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Semight.Fwm.CustomUserControl.WaveLengthParam.Tools;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.Param
{
    public partial class LcParamViewModel : ObservableObject
    {
        [RelayCommand]
        private void Loaded()
        {
            var api = FWM8612Context.GetInstance().WaveAPI;
            if (api != null)
                HardWareManager.RegisterFwmApi(api);
        }

        [RelayCommand]
        private void Unloaded()
        {
        }
    }
}
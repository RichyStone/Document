using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.Fwm8612Helper.Model.SystemSetting;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using System.Collections.ObjectModel;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.SystemSetting
{
    public partial class AdcSettingViewModel : ObservableObject
    {
        #region Fields

        [ObservableProperty]
        private ObservableCollection<SampleSetting>? bindingParam;

        #endregion Fields

        #region Event

        [RelayCommand]
        private void Loaded()
        {
            Initial();
        }

        [RelayCommand]
        private void Unloaded()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        [RelayCommand]
        private void Write()
        {
        }

        #endregion Event

        #region Method

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initial()
        {
            BindingParam = new ObservableCollection<SampleSetting>()
            {
                new() { ChannelIndex=WaveChannel.Ch1, ParamType=SampleParamType.PGA},
                new() { ChannelIndex=WaveChannel.Ch2, ParamType=SampleParamType.PGA},
                new() { ChannelIndex=WaveChannel.Ch1, ParamType=SampleParamType.Offset },
                new() { ChannelIndex=WaveChannel.Ch2, ParamType=SampleParamType.Offset },
            };
        }

        #endregion Method
    }
}
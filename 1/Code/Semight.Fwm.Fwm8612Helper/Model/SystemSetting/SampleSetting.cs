using CommunityToolkit.Mvvm.ComponentModel;
using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;

namespace Semight.Fwm.Fwm8612Helper.Model.SystemSetting
{
    public class SampleSetting : ObservableObject
    {
        private FWM8612Context FwmContext => FWM8612Context.GetInstance();

        public WaveChannel ChannelIndex { get; set; }

        public SampleParamType ParamType { get; set; }

        private int red;

        public int Red
        {
            get => red;
            set
            {
                SetProperty(ref red, value);
                if (red != 0 && FwmContext.Connected)
                    FwmContext.SetSamplingSetting(ChannelIndex, ParamType, ADCType.RED, red);
            }
        }

        private int blue;

        public int Blue
        {
            get => blue; set
            {
                SetProperty(ref blue, value);
                if (blue != 0 && FwmContext.Connected)
                    FwmContext.SetSamplingSetting(ChannelIndex, ParamType, ADCType.BLUE, blue);
            }
        }

        private int green;

        public int Green
        {
            get => green;
            set
            {
                SetProperty(ref green, value);
                if (green != 0 && FwmContext.Connected)
                    FwmContext.SetSamplingSetting(ChannelIndex, ParamType, ADCType.GREEN, green);
            }
        }
    }
}
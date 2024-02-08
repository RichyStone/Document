using System.Collections.Generic;

namespace Semight.Fwm.Common.CommonModels.Classes.InstrumentSetting.InstrumentImplement
{
    public class OSW42032Setting : InstrumentSetting
    {
        public List<ChannelSetting> ChannelSettings { get; set; }
    }

    public class ChannelSetting
    {
        public uint ChannelIndex { get; set; }

        public uint WaveLengthValue { get; set; }
    }
}
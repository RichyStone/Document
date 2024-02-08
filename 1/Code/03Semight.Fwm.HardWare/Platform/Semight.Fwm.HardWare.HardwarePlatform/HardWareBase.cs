using Semight.Fwm.Common.CommonModels.Classes.InstrumentSetting.Connection;

namespace Semight.Fwm.HardWare.HardwarePlatform
{
    public abstract class HardWareBase
    {
        public abstract bool Connected { get; }

        public abstract string InstrumentName { get; }

        public abstract bool HeartBeat();

        public ConnectionSetting ConnectionSetting { get; set; }
    }
}
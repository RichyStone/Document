using Semight.Fwm.Common.CommonModels.Enums;

namespace Semight.Fwm.Common.CommonModels.Classes.InstrumentSetting.Connection
{
    public class NetConnecttionSetting : ConnectionSetting
    {
        public NetConnecttionSetting()
        {
            ConnectionWay = ConnectionWay.Net;
        }

        public string Ip { get; set; }

        public int Port { get; set; }
    }
}
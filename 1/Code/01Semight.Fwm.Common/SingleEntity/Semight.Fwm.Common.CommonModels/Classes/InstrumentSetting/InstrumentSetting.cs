using Semight.Fwm.Common.CommonModels.Classes.InstrumentSetting.Connection;
using Semight.Fwm.Common.CommonModels.Classes.InstrumentSetting.InstrumentImplement;
using Semight.Fwm.Common.CommonModels.Enums;
using System.Xml.Serialization;

namespace Semight.Fwm.Common.CommonModels.Classes.InstrumentSetting
{
    [XmlInclude(typeof(AQ6151Setting))]
    [XmlInclude(typeof(AT4304Setting))]
    [XmlInclude(typeof(HP8153ASetting))]
    [XmlInclude(typeof(HP8164ASetting))]
    [XmlInclude(typeof(OSW42032Setting))]
    [XmlInclude(typeof(ChannelSetting))]
    [XmlInclude(typeof(NetConnecttionSetting))]
    [XmlInclude(typeof(ComConnecttionSetting))]
    [XmlInclude(typeof(USBConnectionSetting))]
    public class InstrumentSetting
    {
        public InstrucmentType InstrucmentType { get; set; }

        public ConnectionSetting ConnectWay { get; set; }
    }
}
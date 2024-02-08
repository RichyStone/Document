namespace Semight.Fwm.Common.CommonModels.Classes.InstrumentSetting.Connection
{
    public class ComConnecttionSetting : ConnectionSetting
    {
        public ComConnecttionSetting()
        {
            ConnectionWay = Enums.ConnectionWay.Com;
        }

        public int ComNo { get; set; }
    }
}
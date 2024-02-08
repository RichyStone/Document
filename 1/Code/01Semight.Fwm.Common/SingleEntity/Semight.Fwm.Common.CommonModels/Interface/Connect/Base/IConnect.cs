namespace Semight.Fwm.Common.CommonModels.Interface.Connect.Base
{
    public interface IConnect
    {
        bool Connected { get; }

        bool HeartBeat();
    }
}
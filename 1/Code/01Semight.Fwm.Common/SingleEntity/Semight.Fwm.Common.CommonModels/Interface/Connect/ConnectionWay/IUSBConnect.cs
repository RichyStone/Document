using Semight.Fwm.Common.CommonModels.Interface.Connect.Base;

namespace Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay
{
    public interface IUSBConnect : IDisConnect
    {
        bool Connect();
    }
}
using Semight.Fwm.Common.CommonModels.Interface.Connect.Base;

namespace Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay
{
    public interface INetConnect : IDisConnect
    {
        bool Connect(string ip, int port);
    }
}
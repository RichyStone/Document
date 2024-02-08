using Semight.Fwm.Common.CommonModels.Interface.Connect.Base;

namespace Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay
{
    public interface IComConnect : IDisConnect
    {
        bool Connect(int com);
    }
}
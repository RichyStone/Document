using System.Threading;

namespace Semight.Fwm.Connection.ConnectionAssistantLib.CommunicateInterface
{
    public interface ICommunicate
    {
        bool Connected { get; }

        bool Connect();

        bool DisConnect();

        void Send(string str);

        void Send(byte[] bytes);

        string ReceiveMessage(CancellationToken cancellationToken);

        byte[] ReceiveData(int length, CancellationToken cancellationToken);
    }
}
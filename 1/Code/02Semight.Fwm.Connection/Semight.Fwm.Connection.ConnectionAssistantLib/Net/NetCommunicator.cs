using Semight.Fwm.Connection.ConnectionAssistantLib.CommunicateInterface;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Semight.Fwm.Connection.ConnectionAssistantLib.Net
{
    public class NetCommunicator : ICommunicate
    {
        private Socket socket;

        private NetworkStream streamToServer;

        public bool Connected => socket.Connected && streamToServer != null;

        private readonly string ipAddress;

        private readonly int port;

        public NetCommunicator(string IP, int Port)
        {
            ipAddress = IP;
            port = Port;
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.NoDelay = true;
                socket.ReceiveTimeout = 1000;

                if (IPCheck(ipAddress))
                {
                    var ip = IPAddress.Parse(ipAddress);
                    socket.Connect(new IPEndPoint(ip, port));

                    if (socket.Connected)
                        streamToServer = new NetworkStream(socket) { ReadTimeout = 3000 };
                }
                else
                    throw new Exception("IPAddress Was Invalid Format！");

                return socket.Connected;
            }
            catch
            {
                socket?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            try
            {
                if (Connected)
                {
                    streamToServer?.Close();
                    streamToServer?.Dispose();
                    socket?.Close();
                    socket?.Dispose();
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="Exception"></exception>
        public void Send(string str)
        {
            try
            {
                if (streamToServer == null || !socket.Connected)
                    throw new Exception("Connection Was Broke！");

                if (string.IsNullOrEmpty(str))
                    throw new Exception("Invalid Message！");

                var data = Encoding.ASCII.GetBytes(str);
                streamToServer.Write(data, 0, data.Length);
                streamToServer.Flush();
                data = null;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="Exception"></exception>
        public void Send(byte[] data)
        {
            try
            {
                if (streamToServer == null || !socket.Connected)
                    throw new Exception("Connection Was Broke！");

                if (data.Length == 0)
                    throw new Exception("Data Size Was Zero！");

                streamToServer.Write(data, 0, data.Length);
                streamToServer.Flush();
                data = null;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ReceiveMessage(CancellationToken cancellationToken)
        {
            try
            {
                if (streamToServer == null || !socket.Connected)
                    throw new Exception("Connection Was Broke！");

                var bytes = new byte[1024];

                int length;
                while ((length = socket.Receive(bytes)) == 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();
                }
                string result = Encoding.ASCII.GetString(bytes, 0, length);
                bytes = null;

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 接收指定数量的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="replyLength"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] ReceiveData(int replyLength, CancellationToken cancellationToken)
        {
            try
            {
                if (streamToServer == null || !socket.Connected)
                    throw new Exception("Connection Was Broke！");

                var bytes = new byte[replyLength];
                int offset = 0;
                while (offset < replyLength)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    var receiveBuffer = new byte[1024];
                    var length = socket.Receive(receiveBuffer);
                    var tempBuffer = receiveBuffer.Take(length).ToArray();
                    tempBuffer.CopyTo(bytes, offset);

                    receiveBuffer = null;
                    tempBuffer = null;
                    offset += length;
                }

                return bytes;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 检查IP格式
        /// IPAddress.TryParse("8", out ip)会得到“0.0.0.8”的输出,所以只能用正则来做
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        private static bool IPCheck(string IP)
        {
            return Regex.IsMatch(IP, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }
}
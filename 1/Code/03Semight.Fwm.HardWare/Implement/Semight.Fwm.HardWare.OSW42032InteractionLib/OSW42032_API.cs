using Semight.Fwm.Connection.ConnectionAssistantLib;
using Semight.Fwm.Connection.ConnectionAssistantLib.Net;
using Semight.Fwm.Connection.ConnectionAssistantLib.Serial;
using Semight.Fwm.Connection.ConnectionAssistantLib.USB;
using Semight.Fwm.HardWare.OSW42032InteractionLib.Command;
using System;

namespace Semight.Fwm.HardWare.OSW42032InteractionLib
{
    public class OSW42032_API
    {
        #region Connection

        public bool Connected { get; private set; }

        public int Vid => Convert.ToInt32("0403", 16);

        public int Pid => Convert.ToInt32("6015", 16);

        /// <summary>
        /// 通讯下下文
        /// </summary>
        private ConnectionContext connectionContext;

        /// <summary>
        /// 开启网口连接
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port)
        {
            try
            {
                var communicateImplement = new NetCommunicator(ip, port);
                connectionContext = new ConnectionContext(communicateImplement);

                return Connected = connectionContext.StartConnect();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Connect(int port)
        {
            try
            {
                var communicateImplement = new SerialPortCommunicator($"Com{port}");
                connectionContext = new ConnectionContext(communicateImplement);

                return Connected = connectionContext.StartConnect();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                Connected = !connectionContext.CloseConnect();

                return !Connected;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Connection

        #region Function

        /// <summary>
        /// 设置远程模式
        /// </summary>
        /// <param name="index">1：串口 2：网口</param>
        public void SetRemoteMode(int index)
        {
            try
            {
                var command = $"{CommandConst.Remote} {index}" + "\n";
                connectionContext.SendMessage(command);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置当前通道
        /// </summary>
        /// <param name="index"></param>
        public void SetChannel(int index)
        {
            try
            {
                var command = $"{CommandConst.SetChannel} {index}" + "\n";
                connectionContext.SendMessage(command);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取当前通道
        /// </summary>
        /// <returns></returns>
        public int QueryCurChannel()
        {
            try
            {
                var command = $"{CommandConst.QueryCurChannel}" + "\n";
                var str = connectionContext.SendMessageWithReply(command);

                if (int.TryParse(str.Trim(), out var channelIndex))
                    return channelIndex;
                else
                    return 0;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset()
        {
            try
            {
                var command = $"{CommandConst.Reset}" + "\n";
                connectionContext.SendMessage(command);
            }
            catch
            {
                throw;
            }
        }

        #endregion Function
    }
}
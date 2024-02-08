using Semight.Fwm.Connection.GP_IBConnectionLib.Enum;
using NationalInstruments.VisaNS;
using System;
using System.Threading;

namespace Semight.Fwm.Connection.GP_IBConnectionLib.GP_IB
{
    public class GPIBCommunicator
    {
        public bool Connected => messageSession != null;

        private MessageBasedSession messageSession;

        private readonly string resourceName;

        public GPIBCommunicator(GP_IBCommunicationMode mode, string connectionStr)
        {
            if (mode == GP_IBCommunicationMode.Address)
                resourceName = "GPIB0::" + connectionStr + "::INSTR";
            else if (mode == GP_IBCommunicationMode.Net)
                resourceName = "TCPIP0::" + connectionStr + "::inst0::INSTR";
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="strIP">IP地址</param>
        /// <param name="Port">端口号</param>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                messageSession = new MessageBasedSession(resourceName);
                return messageSession != null;
            }
            catch
            {
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
                messageSession.Terminate();
                messageSession.Dispose();

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
        /// <param name="msg"></param>
        /// <exception cref="Exception"></exception>
        public void SendMessage(string str)
        {
            try
            {
                if (messageSession == null)
                    throw new Exception("Connection Was Broke！");

                if (string.IsNullOrEmpty(str))
                    throw new Exception("Invalid Message！");

                messageSession.Write(str);
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
        public void SendData(byte[] data)
        {
            try
            {
                if (messageSession == null)
                    throw new Exception("Connection Was Broke！");

                if (data.Length == 0)
                    throw new Exception("Data Size Was Zero！");

                messageSession.Write(data);
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
        public string ReceiveMessage()
        {
            try
            {
                if (messageSession == null)
                    throw new Exception("Connection Was Broke！");

                var result = messageSession.ReadString();

                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="replyLength"></param>
        /// <returns></returns>
        public byte[] ReceiveData(int replyLength)
        {
            try
            {
                if (messageSession == null)
                    throw new Exception("Connection Was Broke！");

                var receiveBuffer = messageSession.ReadByteArray(replyLength);

                return receiveBuffer;
            }
            catch
            {
                throw;
            }
        }
    }
}
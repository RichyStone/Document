using Semight.Fwm.Connection.ConnectionAssistantLib.CommunicateInterface;
using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Semight.Fwm.Connection.ConnectionAssistantLib.Serial
{
    public class SerialPortCommunicator : ICommunicate
    {
        public bool Connected => serialPort != null && serialPort.IsOpen;

        private readonly SerialPort serialPort;

        public SerialPortCommunicator(string portName, int baudRate = 115200, int dataBits = 8, StopBits stopBits = StopBits.One, Parity parity = Parity.None, bool newLine = false, int readTimeOut = 3000, int writeTimeOut = 3000)
        {
            serialPort = new SerialPort(portName, baudRate)
            {
                DataBits = dataBits,
                StopBits = stopBits,
                Parity = parity,
                ReadTimeout = readTimeOut,
                WriteTimeout = writeTimeOut,
            };

            if (newLine)
                serialPort.NewLine = "/r";
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
                if (serialPort == null)
                    throw new Exception("ComDevice Was Null！");

                if (!serialPort.IsOpen)
                    serialPort.Open();

                return serialPort.IsOpen;
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
                if (Connected)
                {
                    if (serialPort != null && serialPort.IsOpen)
                        serialPort.Close();
                }

                return serialPort == null || !serialPort.IsOpen;
            }
            catch
            {
                throw;
            }
            finally
            {
                serialPort.Dispose();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <exception cref="Exception"></exception>
        public void Send(string str)
        {
            try
            {
                if (serialPort == null || !serialPort.IsOpen)
                    throw new Exception("Connection Was Broke！");

                if (string.IsNullOrEmpty(str))
                    throw new Exception("Invalid Message！");

                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.WriteLine(str);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        /// <exception cref="Exception"></exception>
        public void Send(byte[] data)
        {
            try
            {
                if (serialPort == null || !serialPort.IsOpen)
                    throw new Exception("Connection Was Broke！");

                if (data.Length == 0)
                    throw new Exception("Data Size Was Zero！");

                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.Write(data, 0, data.Length);
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
                if (serialPort == null || !serialPort.IsOpen)
                    throw new Exception("Connection Was Broke！");

                int icycle = 0;
                string str = "\r";
                string result = null;
                do
                {
                    icycle++;
                    result = serialPort.ReadTo(str);

                    if (icycle > 500)
                        break;
                }
                while (result.Trim('\r') == "" && !cancellationToken.IsCancellationRequested);

                if (result.Contains("\r"))
                    result = result.Trim(str.ToCharArray());

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
        public byte[] ReceiveData(int replyLength, CancellationToken cancellationToken)
        {
            try
            {
                if (serialPort == null || !serialPort.IsOpen)
                    throw new Exception("Connection Was Broke！");

                var bytes = new byte[replyLength];
                int offset = 0;
                while (offset < replyLength)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    var receiveBuffer = new byte[1024];
                    var length = serialPort.Read(receiveBuffer, 0, receiveBuffer.Length);
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
    }
}
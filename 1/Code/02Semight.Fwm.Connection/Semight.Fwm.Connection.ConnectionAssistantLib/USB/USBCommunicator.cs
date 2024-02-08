using Semight.Fwm.Connection.ConnectionAssistantLib.CommunicateInterface;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Semight.Fwm.Connection.ConnectionAssistantLib.USB
{
    public class USBCommunicator : ICommunicate
    {
        private UsbDevice usbDevice;
        private UsbEndpointReader reader = null;
        private UsbEndpointWriter writer = null;

        private readonly int vendorID;
        private readonly int productID;
        private readonly string serialString;

        public bool Connected => usbDevice != null && usbDevice.IsOpen;

        public USBCommunicator(int vid, int pid, string serial = "")
        {
            vendorID = vid;
            productID = pid;
            serialString = serial;
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <param name="strIP">IP地址</param>
        /// <param name="Port">端口号</param>
        /// <returns></returns>
        public bool Connect()
        {
            var usbFinder = string.IsNullOrEmpty(serialString) ? new UsbDeviceFinder(vendorID, productID) : new UsbDeviceFinder(vendorID, productID, serialString.ToLower());
            try
            {
                int tryTime = 0;
                while (tryTime < 10 && usbDevice == null)
                {
                    usbDevice = UsbDevice.OpenUsbDevice(usbFinder);
                    Thread.Sleep(100);
                    tryTime++;
                }

                if (usbDevice == null) throw new Exception("Device Not Found.");

                if (usbDevice is IUsbDevice wholeUsbDevice)
                {
                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    wholeUsbDevice.ClaimInterface(0);
                }

                // open read endpoint 1.
                reader = usbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

                // open write endpoint 1.
                writer = usbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);

                return true;
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
                    if (usbDevice is IUsbDevice wholeUsbDevice)
                    {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(0);
                    }

                    usbDevice.Close();
                    usbDevice = null;
                }

                // Free usb resource
                ////UsbDevice.Exit();

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
        public void Send(string str)
        {
            try
            {
                if (usbDevice == null || !usbDevice.IsOpen)
                    throw new Exception("Connection Was Broke！");

                if (string.IsNullOrEmpty(str))
                    throw new Exception("Invalid Message！");

                var data = Encoding.ASCII.GetBytes(str);
                ErrorCode ec = writer.Write(data, 1000, out int byteWrite);
                if (ec != ErrorCode.None)
                    throw new Exception($"Write Error, {UsbDevice.LastErrorString}");
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
                if (usbDevice == null || !usbDevice.IsOpen)
                    throw new Exception("Connection Was Broke！");

                ErrorCode ec = writer.Write(data, 1000, out int byteWrite);
                if (ec != ErrorCode.None)
                    throw new Exception($"Write Error, {UsbDevice.LastErrorString}");
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
                if (usbDevice == null || !usbDevice.IsOpen)
                    throw new Exception("Connection Was Broke！");

                int byteRead = 0;
                byte[] readBuffer = new byte[1024];
                while (byteRead == 0)
                {
                    ErrorCode ec = reader.Read(readBuffer, 1000, out byteRead);
                    if (ec != ErrorCode.None)
                        throw new Exception($"Read Error, {UsbDevice.LastErrorString}");

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();
                }

                // 只有当超时的时候才会有byteRead为0，也就是结束
                string result = Encoding.ASCII.GetString(readBuffer, 0, byteRead);

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
                if (usbDevice == null || !usbDevice.IsOpen)
                    throw new Exception("Connection Was Broke！");

                var bytes = new byte[replyLength];
                int offset = 0;
                while (offset < replyLength)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    var receiveBuffer = new byte[1024];
                    var ec = reader.Read(receiveBuffer, 1000, out int length);
                    if (ec != ErrorCode.None)
                        throw new Exception($"Read Error, {UsbDevice.LastErrorString}");

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
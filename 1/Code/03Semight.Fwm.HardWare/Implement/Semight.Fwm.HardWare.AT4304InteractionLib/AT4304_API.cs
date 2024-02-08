using System;
using System.Threading;

namespace Semight.Fwm.HardWare.AT4304InteractionLib
{
    public class AT4304_API
    {
        public uint DeviceID = 0;

        private readonly Inst_ST_OptAtt optAtt = new Inst_ST_OptAtt();

        public AT4304_API(uint deviceId)
        {
            DeviceID = deviceId;
        }

        #region Connection

        public bool Connected { get; private set; }

        /// <summary>
        /// 开启网口连接
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port, int timeOut = 2000)
        {
            try
            {
                var res = optAtt.OpenDevice(DeviceID, ip, port);
                if (!res) return Connected = false;

                var thread = new Thread(() => { QueryProductName(); }) { IsBackground = true };
                thread.Start();

                if (!thread.JudgeTimeOut(timeOut))
                    return Connected = false;

                return Connected = true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 开启USB连接
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        public bool Connect(uint port, int timeOut = 2000)
        {
            try
            {
                var res = optAtt.OpenDevice(DeviceID, port);
                if (!res) return Connected = false;

                var thread = new Thread(() => { QueryProductName(); }) { IsBackground = true };
                thread.Start();

                if (!thread.JudgeTimeOut(timeOut))
                    return Connected = false;

                return Connected = true;
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
        public bool Close()
        {
            try
            {
                if (Connected)
                {
                    var thread = new Thread(() => { optAtt.CloseDevice(DeviceID); }) { IsBackground = true };
                    thread.Start();

                    if (!thread.JudgeTimeOut(2000))
                        return false;

                    Connected = false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Connection

        #region Function

        public void SetAttenuation(uint channel, float att)
        {
            optAtt.SetAttenuation(DeviceID, channel, att);
        }

        public double QueryAttenuation(uint channel)
        {
            return optAtt.QueryAttenuation(DeviceID, channel);
        }

        public string QueryHardwareIdentification()
        {
            return optAtt.HardwareIdentification(DeviceID);
        }

        public string QueryProductName()
        {
            return optAtt.QueryProductName(DeviceID);
        }

        public string QueryIp()
        {
            return optAtt.QueryIPaddress(DeviceID);
        }

        public ushort QueryPort()
        {
            return optAtt.QueryNetPort(DeviceID);
        }

        public void SetIp(string ipAddress)
        {
            optAtt.SetIPaddress(DeviceID, ipAddress);
        }

        public void SetPort(ushort port)
        {
            optAtt.SetNetPort(DeviceID, port);
        }

        #endregion Function
    }

    public static class TimeOutAdapter
    {
        public static bool JudgeTimeOut(this Thread thread, int TimeOout)
        {
            if (!thread.Join(TimeOout))
            {
                thread.Interrupt();
                return false;
            }
            else
                return true;
        }

        public static bool JudgeTimeOut(this Thread thread, TimeSpan TimeOout)
        {
            if (!thread.Join(TimeOout))
            {
                thread.Interrupt();
                return false;
            }
            else
                return true;
        }
    }
}
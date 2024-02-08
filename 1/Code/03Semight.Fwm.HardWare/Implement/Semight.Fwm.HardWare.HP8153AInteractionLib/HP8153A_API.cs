using Semight.Fwm.Connection.GP_IBConnectionLib.Enum;
using Semight.Fwm.Connection.GP_IBConnectionLib.GP_IB;
using Semight.Fwm.HardWare.HP8153AInteractionLib.Command;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Semight.Fwm.HardWare.HP8153AInteractionLib
{
    public class HP8153A_API
    {
        #region Connection

        public bool Connected { get; private set; } = false;

        private GPIBCommunicator communicator = null;

        public bool Connect(GP_IBCommunicationMode mode, string connectionStr)
        {
            try
            {
                communicator = new GPIBCommunicator(mode, connectionStr);
                if (communicator.Connect())
                {
                    ReadPower();
                    Connected = true;
                }
                else
                    Connected = false;

                return Connected;
            }
            catch
            {
                throw;
            }
        }

        public bool Close()
        {
            try
            {
                Connected = !communicator.DisConnect();

                return !Connected;
            }
            catch
            {
                throw;
            }
        }

        #endregion Connection

        #region Function

        /// <summary>
        /// 读取功率
        /// </summary>
        /// <returns></returns>
        public double ReadPower()
        {
            try
            {
                var command = $"{CommandConst.Power}?";
                communicator.SendMessage(command);

                Thread.Sleep(20);
                var str = communicator.ReceiveMessage();

                if (double.TryParse(str.Trim(), out var power))
                    return power;
                else
                    return 0.0;
            }
            catch
            {
                throw;
            }
        }

        public string ReadPowerUnit()
        {
            try
            {
                var command = $"{CommandConst.PowerUnit}?";
                communicator.SendMessage(command);

                Thread.Sleep(20);
                var str = communicator.ReceiveMessage().Trim() == "0" ? "DBm" : "mW";

                return str;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置功率单位
        /// </summary>
        /// <param name="index">0为DBm,1为mW</param>
        public void SetPowerUnit(int index)
        {
            try
            {
                var command = $"{CommandConst.PowerUnit} {index}";
                communicator.SendMessage(command);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 读取波长，单位为nm
        /// </summary>
        /// <returns></returns>
        public double ReadWaveLength()
        {
            try
            {
                var command = $"{CommandConst.WaveLength}?";
                communicator.SendMessage(command);

                Thread.Sleep(20);
                var str = communicator.ReceiveMessage();

                if (double.TryParse(str.Trim(), out var wavelength))
                    return wavelength;
                else
                    return 0.0;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 设置波长，单位为nm
        /// </summary>
        /// <param name="waveLength"></param>
        public void SetWaveLength(double waveLength)
        {
            try
            {
                var command = $"{CommandConst.WaveLength} {waveLength}NM";
                communicator.SendMessage(command);
                Thread.Sleep(20);
            }
            catch
            {
                throw;
            }
        }

        #endregion Function
    }
}
using Semight.Fwm.Connection.GP_IBConnectionLib.Enum;
using Semight.Fwm.Connection.GP_IBConnectionLib.GP_IB;
using Semight.Fwm.HardWare.HP8164AInteractionLib.Command;
using System;
using System.Threading;

namespace Semight.Fwm.HardWare.HP8164AInteractionLib
{
    public class HP8164A_API
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
                    Read_Wavelength();
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

        public void Set_Wavelength(double WL)//nm
        {
            try
            {
                //1510~1640
                if (WL < 1510) WL = 1510;
                else if (WL > 1640) WL = 1640;

                string temp = (WL / 1E9).ToString();
                string CMD = CommandConst.CMD_SetWL + temp;
                communicator.SendMessage(CMD);
            }
            catch
            {
                throw;
            }
        }

        public double Read_Wavelength()
        {
            try
            {
                communicator.SendMessage(CommandConst.CMD_QueryWL);
                Thread.Sleep(20);
                var str = communicator.ReceiveMessage();

                if (double.TryParse(str.Trim(), out var waveLength))
                    return waveLength * 1E9;
                else
                    return 0.0;
            }
            catch
            {
                throw;
            }
        }

        public void Set_Power(double power)
        {
            try
            {
                string CMD = CommandConst.CMD_SetPower + power;
                communicator.SendMessage(CMD);
            }
            catch
            {
                throw;
            }
        }

        public double Read_Power()
        {
            try
            {
                string CMD = CommandConst.CMD_ReadPower;
                communicator.SendMessage(CMD);

                Thread.Sleep(20);
                var result = communicator.ReceiveMessage();
                if (double.TryParse(result.Trim(), out var power))
                    return power;
                else
                    return 0.0;
            }
            catch
            {
                throw;
            }
        }

        #endregion Function
    }
}
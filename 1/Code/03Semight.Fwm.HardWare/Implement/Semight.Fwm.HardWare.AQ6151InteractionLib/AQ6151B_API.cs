using Semight.Fwm.Connection.GP_IBConnectionLib.Enum;
using Semight.Fwm.Connection.GP_IBConnectionLib.GP_IB;
using Semight.Fwm.HardWare.AQ6151InteractionLib.Command;
using System.Threading;

namespace Semight.Fwm.HardWare.AQ6151InteractionLib
{
    public class AQ6151B_API
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
                    Connected = true;
                    Read_Wavelength();
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

        public double Read_Wavelength()//nm
        {
            try
            {
                communicator.SendMessage(CommandConst.ReadWaveLength);

                Thread.Sleep(20);
                var wlStr = communicator.ReceiveMessage();
                double result = 0;
                if (double.TryParse(wlStr.Trim(), out double wl))
                    result = wl * 1E9;

                return result;
            }
            catch
            {
                throw;
            }
        }

        #endregion Function
    }
}
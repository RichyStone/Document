using Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay;
using Semight.Fwm.HardWare.AQ6151InteractionLib;
using System;
using System.Threading.Tasks;

namespace Semight.Fwm.HardWare.HardwarePlatform.AQ6151
{
    public sealed class AQ6151Context : HardWareBase, INetConnect, IComConnect
    {
        #region Fields

        /// <summary>
        /// AQ6151API
        /// </summary>
        private readonly AQ6151B_API aq6151BAPI = new AQ6151B_API();

        /// <summary>
        /// 连接状态
        /// </summary>
        public override bool Connected => aq6151BAPI.Connected;

        public override string InstrumentName => "AQ6151B";

        #endregion Fields

        #region Instance

        private static readonly Lazy<AQ6151Context> _instance = new Lazy<AQ6151Context>(() => new AQ6151Context());

        public static AQ6151Context GetInstance() => _instance.Value;

        private AQ6151Context()
        {
        }

        #endregion Instance

        #region Methods

        #region Connection

        /// <summary>
        /// GPIB串口连接
        /// </summary>
        /// <param name="comNo"></param>
        /// <returns></returns>
        public bool Connect(int comNo) => aq6151BAPI.Connect(Fwm.Connection.GP_IBConnectionLib.Enum.GP_IBCommunicationMode.Address, $"{comNo}");

        /// <summary>
        /// GPIB网口连接
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port = 8807) => aq6151BAPI.Connect(Fwm.Connection.GP_IBConnectionLib.Enum.GP_IBCommunicationMode.Net, ip);

        /// <summary>
        /// 断连
        /// </summary>
        /// <returns></returns>
        public bool DisConnect() => aq6151BAPI.Close();

        /// <summary>
        /// 心跳
        /// </summary>
        /// <returns></returns>
        public override bool HeartBeat()
        {
            try
            {
                return Task.Run(async () =>
               {
                   for (int i = 0; i < 3; i++)
                   {
                       ReadWaveLength();
                       await Task.Delay(300);
                   }

                   return true;
               }).GetAwaiter().GetResult();
            }
            catch
            {
                return false;
            }
        }

        #endregion Connection

        #region Function

        /// <summary>
        /// 读取波长
        /// </summary>
        /// <returns></returns>
        public double ReadWaveLength() => aq6151BAPI.Read_Wavelength();

        #endregion Function

        #endregion Methods
    }
}
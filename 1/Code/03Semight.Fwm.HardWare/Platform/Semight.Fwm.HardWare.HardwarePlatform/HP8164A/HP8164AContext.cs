using Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay;
using Semight.Fwm.HardWare.HP8164AInteractionLib;
using System;
using System.Threading.Tasks;

namespace Semight.Fwm.HardWare.HardwarePlatform.HP8164A
{
    public sealed class HP8164AContext : HardWareBase, INetConnect, IComConnect
    {
        #region Fields

        private readonly HP8164A_API hp8164A = new HP8164A_API();

        public override bool Connected => hp8164A.Connected;

        public override string InstrumentName => "HP8164A";

        #endregion Fields

        #region Instance

        private HP8164AContext()
        {
        }

        private static readonly Lazy<HP8164AContext> context = new Lazy<HP8164AContext>(() => new HP8164AContext());

        public static HP8164AContext GetInstance() => context.Value;

        #endregion Instance

        #region Method

        #region Connection

        /// <summary>
        /// 串口连接
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public bool Connect(int com) => hp8164A.Connect(Fwm.Connection.GP_IBConnectionLib.Enum.GP_IBCommunicationMode.Address, $"{com}");

        /// <summary>
        /// 网口连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port) => hp8164A.Connect(Fwm.Connection.GP_IBConnectionLib.Enum.GP_IBCommunicationMode.Net, ip);

        /// <summary>
        /// 断连
        /// </summary>
        /// <returns></returns>
        public bool DisConnect() => hp8164A.Close();

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
                        ReadPower();
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
        /// 设置波长
        /// </summary>
        /// <param name="WL"></param>
        public void SetWavelength(double WL) => hp8164A.Set_Wavelength(WL);

        public double ReadWavelength() => hp8164A.Read_Wavelength();

        public void SetPower(double power) => hp8164A.Set_Power(power);

        public double ReadPower() => hp8164A.Read_Power();

        #endregion Function

        #endregion Method
    }
}
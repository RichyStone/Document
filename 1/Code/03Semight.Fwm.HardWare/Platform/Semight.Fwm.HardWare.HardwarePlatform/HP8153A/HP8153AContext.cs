using Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay;
using Semight.Fwm.HardWare.HP8153AInteractionLib;
using System;
using System.Threading.Tasks;

namespace Semight.Fwm.HardWare.HardwarePlatform.HP8153A
{
    public sealed class HP8153AContext : HardWareBase, INetConnect, IComConnect
    {
        #region Fields

        private readonly HP8153A_API hp8153A = new HP8153A_API();

        public override bool Connected => hp8153A.Connected;

        public override string InstrumentName => "HP8153A";

        #endregion Fields

        #region Instance

        private HP8153AContext()
        {
        }

        private static readonly Lazy<HP8153AContext> context = new Lazy<HP8153AContext>(() => new HP8153AContext());

        public static HP8153AContext GetInstance() => context.Value;

        #endregion Instance

        #region Method

        #region Connection

        /// <summary>
        /// 串口连接
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public bool Connect(int com) => hp8153A.Connect(Fwm.Connection.GP_IBConnectionLib.Enum.GP_IBCommunicationMode.Address, $"{com}");

        /// <summary>
        /// 网口连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port) => hp8153A.Connect(Fwm.Connection.GP_IBConnectionLib.Enum.GP_IBCommunicationMode.Net, ip);

        /// <summary>
        /// 断连
        /// </summary>
        /// <returns></returns>
        public bool DisConnect() => hp8153A.Close();

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
        /// 读取功率
        /// </summary>
        /// <returns></returns>
        public double ReadPower() => hp8153A.ReadPower();

        public string ReadPowerUnit() => hp8153A.ReadPowerUnit();

        /// <summary>
        /// 设置功率单位
        /// </summary>
        /// <param name="index">0为DBm,1为mW</param>
        public void SetPowerUnit(int index) => hp8153A.SetPowerUnit(index);

        /// <summary>
        /// 读取波长，单位为nm
        /// </summary>
        /// <returns></returns>
        public double ReadWaveLength() => hp8153A.ReadWaveLength();

        /// <summary>
        /// 设置波长，单位为nm
        /// </summary>
        /// <param name="waveLength"></param>
        public void SetWaveLength(double waveLength) => hp8153A.SetWaveLength(waveLength);

        #endregion Function

        #endregion Method
    }
}
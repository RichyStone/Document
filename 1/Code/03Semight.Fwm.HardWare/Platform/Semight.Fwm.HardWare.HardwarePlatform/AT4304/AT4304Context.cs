using Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay;
using Semight.Fwm.HardWare.AT4304InteractionLib;
using System;
using System.Threading.Tasks;

namespace Semight.Fwm.HardWare.HardwarePlatform.AT4304
{
    public sealed class AT4304Context : HardWareBase, INetConnect, IComConnect
    {
        #region Fields

        private readonly AT4304_API at4304API = new AT4304_API(0);

        public override bool Connected => at4304API.Connected;
        public override string InstrumentName => "AT4304";

        private uint Channel { get; set; } = 1;

        #endregion Fields

        #region Instance

        private AT4304Context()
        {
        }

        private static readonly Lazy<AT4304Context> context = new Lazy<AT4304Context>(() => new AT4304Context());

        public static AT4304Context GetInstance() => context.Value;

        #endregion Instance

        #region Method

        #region Connection

        /// <summary>
        /// 串口连接
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public bool Connect(int com) => at4304API.Connect((uint)com);

        /// <summary>
        /// 网口连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port) => at4304API.Connect(ip, port);

        /// <summary>
        /// 断连
        /// </summary>
        /// <returns></returns>
        public bool DisConnect() => at4304API.Close();

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
                        GetProductName();
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
        /// 设置输出功率
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="power"></param>
        public void SetAttenuation(float power) => at4304API.SetAttenuation(Channel, power);

        /// <summary>
        /// 获取输出功率
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public double QueryAttenuation() => at4304API.QueryAttenuation(Channel);

        /// <summary>
        /// 获取产品名
        /// </summary>
        /// <returns></returns>
        public string GetProductName() => at4304API.QueryProductName();

        #endregion Function

        #endregion Method
    }
}
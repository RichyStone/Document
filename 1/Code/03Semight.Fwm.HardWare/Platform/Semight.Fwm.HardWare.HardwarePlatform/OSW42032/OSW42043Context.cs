using Semight.Fwm.Common.CommonModels.Interface.Connect.ConnectionWay;
using Semight.Fwm.HardWare.OSW42032InteractionLib;
using System;
using System.Threading.Tasks;

namespace Semight.Fwm.HardWare.HardwarePlatform.OSW42032
{
    public sealed class OSW42043Context : HardWareBase, INetConnect, IComConnect
    {
        #region Fields

        private readonly OSW42032_API osw42043 = new OSW42032_API();

        public override bool Connected => osw42043.Connected;

        public override string InstrumentName => "OSW42032";

        /// <summary>
        /// 远程索引
        /// </summary>
        private int RemoteIndex = 0;

        #endregion Fields

        #region Instance

        private OSW42043Context()
        {
        }

        private static readonly Lazy<OSW42043Context> context = new Lazy<OSW42043Context>(() => new OSW42043Context());

        public static OSW42043Context GetInstance() => context.Value;

        #endregion Instance

        #region Method

        #region Connection

        public bool Connect(int com)
        {
            bool res;
            if (res = osw42043.Connect(com))
                RemoteIndex = 1;
            return res;
        }

        public bool Connect(string ip, int port)
        {
            bool res;
            if (res = osw42043.Connect(ip, port))
                RemoteIndex = 2;
            return res;
        }

        public bool DisConnect() => osw42043.Close();

        public override bool HeartBeat()
        {
            try
            {
                return Task.Run(async () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        GetCurChannel();
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
        /// 设置远程模式
        /// </summary>
        /// <param name="index">1：串口 2：网口</param>
        public void SetRemoteMode() => osw42043.SetRemoteMode(RemoteIndex);

        /// <summary>
        /// 设置当前通道
        /// </summary>
        /// <param name="index"></param>
        public void SetChannel(int index) => osw42043.SetChannel(index);

        /// <summary>
        /// 获取当前通道
        /// </summary>
        /// <returns></returns>
        public int GetCurChannel() => osw42043.QueryCurChannel();

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset() => osw42043.Reset();

        #endregion Function

        #endregion Method
    }
}
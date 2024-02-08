using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.Common.CommonTools.ConfigueAppSetting;
using Semight.Fwm.Fwm8612Helper.Model.SystemSetting;
using Semight.Fwm.Fwm8612Helper.Model.WaveLengthChartSetting;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;

namespace Semight.Fwm.Fwm8612Helper.CommonBusiness.Config
{
    public class GlobalConfig
    {
        /// <summary>
        /// 触发模式
        /// </summary>
        public static FWMTriggerMode TriggerType { get; set; }

        /// <summary>
        /// 计算平均数
        /// </summary>
        public static int Average { get; set; } = 20;

        /// <summary>
        /// 波长单位
        /// </summary>
        public static WaveLengthUnit WaveLengthUnit { get; set; }

        /// <summary>
        /// 功率单位
        /// </summary>
        public static PowerUnit PowerUnit { get; set; }

        /// <summary>
        /// 腔体薄厚
        /// </summary>
        public static CavityType CavityType { get; set; }

        /// <summary>
        /// 连接方式
        /// </summary>
        public static ConnectionWay ConnectionWay { get; set; } = ConnectionWay.Net;

        /// <summary>
        /// IP
        /// </summary>
        public static string IP { get; set; } = ConfigHelper.GetAppSetting("LCIPAddress", "192.168.170.7");

        /// <summary>
        /// 端口号
        /// </summary>
        public static int Port { get; set; } = ConfigHelper.GetAppSettingToInt("LCPort", 8807);

        /// <summary>
        /// 波长图设定
        /// </summary>
        public static WaveLengthChartSetting ChartSetting { get; set; } = new WaveLengthChartSetting();

        /// <summary>
        /// 软件设定
        /// </summary>
        public static SoftwareSetting SoftwareSetting { get; set; } = new SoftwareSetting();
    }
}
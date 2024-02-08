using Semight.Fwm.Common.CommonModels.Enums;

namespace Semight.Fwm.Common.CommonModels.Classes.Param
{
    /// <summary>
    /// 参考光源参数
    /// </summary>
    public class FreqStableParam
    {
        public WaveBand WaveBand { get; set; }

        public double WL_Step1 { get; set; }

        public double WL_Step2 { get; set; }

        public double WL_Final { get; set; }
    }
}
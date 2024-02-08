using Semight.Fwm.Common.CommonModels.Enums;
using System.Collections.Generic;

namespace Semight.Fwm.Common.CommonModels.Classes.Param
{
    /// <summary>
    /// 多峰参数
    /// </summary>
    public class MultiPeaksParam
    {
        public bool MultiPeaksFlag { get; set; }

        public bool SIDMultiPeaksFlag { get; set; }

        public List<PeakParam> Peaks { get; set; } = new List<PeakParam>();
    }

    public class PeakParam
    {
        public WaveChannel ChannelIndex { get; set; }

        public double Node { get; set; }

        public double Nd_th { get; set; } = 0.2;

        public double Pv_th { get; set; } = 1500;

        public double Nd_sth { get; set; } = 0.8;
    }
}
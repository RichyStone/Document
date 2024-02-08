using Semight.Fwm.Common.CommonModels.Enums;
using System.Collections.Generic;

namespace Semight.Fwm.Common.CommonModels.Classes.Param
{
    /// <summary>
    /// 色散补偿参数
    /// </summary>
    public class DispersionCompensateParam
    {
        public bool FitStatus { get; set; }

        public List<CompensationParam> CompensationParam { get; set; } = new List<CompensationParam>();
    }

    public class CompensationParam
    {
        public WaveBand WaveBand { get; set; }

        public CavityType CavityType { get; set; }

        public double K1 { get; set; }

        public double K2 { get; set; }

        public double B { get; set; }
    }
}
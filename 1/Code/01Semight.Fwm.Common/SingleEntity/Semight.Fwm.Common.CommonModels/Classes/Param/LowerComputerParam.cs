using System.Collections.Generic;

namespace Semight.Fwm.Common.CommonModels.Classes.Param
{
    /// <summary>
    /// 下位机参数
    /// </summary>
    public class LowerComputerParam
    {
        /// <summary>
        /// 功率校准参数
        /// </summary>
        public PowerParam PowerParam { get; set; } = new PowerParam();

        /// <summary>
        /// 多峰参数
        /// </summary>
        public MultiPeaksParam MultiPeaksParam { get; set; } = new MultiPeaksParam();

        /// <summary>
        /// 菲索参数
        /// </summary>
        public List<FizeauParam> FizeauParameters { get; set; } = new List<FizeauParam>();

        /// <summary>
        /// 补偿参数
        /// </summary>
        public DispersionCompensateParam DispersionCompensation { get; set; } = new DispersionCompensateParam();

        /// <summary>
        /// 参考光源参数
        /// </summary>
        public List<FreqStableParam> FreqStableWLParam { get; set; } = new List<FreqStableParam>();
    }
}
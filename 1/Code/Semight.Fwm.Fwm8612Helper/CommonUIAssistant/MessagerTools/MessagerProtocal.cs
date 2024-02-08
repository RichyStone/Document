namespace Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools
{
    public class MessagerProtocal
    {
        /// <summary>
        /// 连接状态改变
        /// </summary>
        public const string ConnectState = "ConnectionChanged";

        /// <summary>
        /// 测量状态改变
        /// </summary>
        public const string MeasureState = "MeasureStateChanged";

        /// <summary>
        /// 刷新硬件设置
        /// </summary>
        public const string RefreshHardWareSettings = "HardWareSetting";

        /// <summary>
        /// 上位机计算波长数据
        /// </summary>
        public const string CalculateTypeChanged = "WaveLengthSelfCalculate";

        /// <summary>
        /// 改变波长单位
        /// </summary>
        public const string WavelengthUnitChanged = "WavelengthUnitChanged";

        /// <summary>
        /// 改变功率单位
        /// </summary>
        public const string PowerUnitChanged = "PowerUnitChanged";

        /// <summary>
        /// 增益状态改变
        /// </summary>
        public const string GainStateChanged = "GainStateChanged";
    }
}
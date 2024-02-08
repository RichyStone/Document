namespace Semight.Fwm.HardWare.FWM8612InteractionLib.Command
{
    public class CommandConst
    {
        #region CommonFix

        public const string WaveBandFix = "BANd";

        public const string ThinFix = "THIN";

        public const string ThickFix = "THICK";

        #endregion CommonFix

        public const string QuerySuccess = "*OPC";

        public const string OpenFPGAPower = "SYSTem:INSTrument:POWer";
        public const string OpenCCDADC = "CONFigure:CCDSAMPle:ENAble";
        public const string MacDefault = "SYSTem:INSTrument:INIT";

        public const string QueryIDN = "*IDN";

        public const string WaveBand = "CONFigure:BAND";

        public const string CCDSample = "READ:CCDSAMPle:AD";

        #region IPAddress

        public const string FlashIp = ":SYST:COMM:LAN:ADDR";
        public const string FlashMask = ":SYST:COMM:LAN:SMAS";
        public const string FlashGate = ":SYST:COMM:LAN:GAT";
        public const string FlashPort = ":SYST:COMM:LAN:PORT";

        #endregion IPAddress

        #region 自动校准

        public const string AutoCalibration = ":CALibrate:AUTO";
        public const string AutoCalibrationCycle = ":CALibrate:AUTO:CYCLe";
        public const string LastCalibrationTime = ":CALibrate:AUTO:LASTime";

        #endregion 自动校准

        #region 出厂设置

        public const string FactorySave = "FACTory:SAVE";
        public const string Rst = "*RST";

        #endregion 出厂设置

        #region 切换光源

        public const string SetLaserIn = ":CALIbrate:LSOurce IN";
        public const string SetLaserOut = ":CALIbrate:LSOurce OUT";

        #endregion 切换光源

        #region 触发模式

        public const string TriggerMode = "TRIGger:SOURce";

        public const string ExternalMode = "TRIGger:SOURce EXTernal";
        public const string InternalMode = "TRIGger:SOURce INTernal";
        public const string SingleMode = "TRIGger:SOURce SINgle";

        public const string TriggerFrequency = "CONFigure:INTernal:FREQuency";

        #endregion 触发模式

        #region 曝光时间

        public const string FPGA_IntergrationTime = ":CONFigure:EXPOsure";
        public const string FPGA_ExpoTimeAuto = ":CONFigure:EXPOsure:Auto";
        public const string FPGA_ExpoTimeAdaptive = "CONFigure:SINGle:AUTO";

        #endregion 曝光时间

        #region Gain

        public const string Gain = ":CONF:GAIN";

        public const string GainHighState = "High";

        public const string GainLowState = "Low";

        #endregion Gain

        #region ADC

        public const string ADCSetting = ":CONFigure:ADC";
        public const string ADCPGA = "PGA";
        public const string ADCOffset = "OFFSET";

        #endregion ADC

        #region 参数

        public const string GetParamValidity = ":CALibrate:VALidity";

        public const string SaveParam = ":CALibrate:SAVE";

        public const string Fizeau = ":CALibrate:FIZeau";

        public const string Compensation = ":CALibrate:DCOMpensate";
        public const string CompensationStatus = ":CALibrate:DCOMpensate:STATus";

        public const string PowerCompensation = ":CALibrate:POWer:PARA";

        public const string MultiPeakStatus = ":CALibrate:MPAeks:STATus";
        public const string MultiPeakSIDStatus = ":CALibrate:MPAeks:SID";
        public const string MultiPeakNode = ":CALibrate:MPAeks:NODe";
        public const string MultiPeakNDTH = ":CALibrate:MPAeks:NDTH";
        public const string MultiPeakPVTH = ":CALibrate:MPAeks:PVTH";
        public const string MultiPeakNDSTH = ":CALibrate:MPAeks:NDST";

        public const string FSWL = ":CALibrate:FSWL";

        #endregion 参数

        #region 测量

        public const string MeasureWLAndPower = "MEAS:WPOW";
        public const string MeasureWaveLength = "MEASure:WL";
        public const string MeasurePower = "MEASure:POWer";

        public const string PowerUnit = "CONFigure:POWer:UNIT";
        public const string WaveLengthUnit = "CONFigure:WAVelength:UNIT";

        public const string AverageCount = "CONFigure:FILTer:COUNt";

        #endregion 测量

        #region Temperature

        public const string GetTCM_HeaterState = "READ:TCM:HEATerstate";
        public const string GetTCM_Temperature = "READ:TCM";

        public const string TemperatureFix = "TEMPerature";

        #endregion Temperature
    }
}
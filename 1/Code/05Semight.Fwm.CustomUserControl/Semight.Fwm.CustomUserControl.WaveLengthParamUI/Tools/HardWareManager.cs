using Semight.Fwm.Common.CommonModels.Classes.Param;
using Semight.Fwm.HardWare.FWM8612InteractionLib;
using System.Collections.Generic;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.Tools
{
    public class HardWareManager
    {
        private static FWM8612_API? waveTestApi = null;

        public static bool Connected => waveTestApi != null && waveTestApi.Connected;

        public static void RegisterFwmApi(FWM8612_API waveApi)
        {
            waveTestApi = waveApi;
        }

        public static void WriteCompensationParam(DispersionCompensateParam param)
        {
            if (Connected)
                waveTestApi?.SetDispersionCompensate(param);
        }

        public static void WriteFizeauParam(List<FizeauParam> param)
        {
            if (Connected)
                waveTestApi?.SetFizeauParam(param);
        }

        public static void WriteFreqStableParam(List<FreqStableParam> param)
        {
            if (Connected)
                waveTestApi?.SetFreqStableParam(param);
        }

        public static void WriteMultiPeaksParam(MultiPeaksParam param)
        {
            if (Connected)
                waveTestApi?.SetMultiPeaksParam(param);
        }

        public static void WritePowerParam(PowerParam param)
        {
            if (Connected)
                waveTestApi?.SetPowerCompensate(param);
        }

        public static bool ReadLowerCpParam()
        {
            if (!Connected)
                return false;

            waveTestApi?.GetMultiPeaksParam();
            waveTestApi?.GetPowerCompensate();
            waveTestApi?.GetDispersionCompensate();
            waveTestApi?.GetFizeau();
            waveTestApi?.GetFreqStableParam();

            return true;
        }

        public static void SaveToLower()
        {
            if (Connected)
                waveTestApi?.CalibrateSAVE();
        }

        public static LowerComputerParam? GetLowerComputerParam()
        {
            if (Connected)
                return waveTestApi?.ReadLowerComputerParam();
            else
                return null;
        }
    }
}
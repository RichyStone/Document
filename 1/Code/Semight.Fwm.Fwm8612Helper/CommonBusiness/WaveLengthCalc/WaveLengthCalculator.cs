using Semight.Fwm.Algorithm.WaveLengthAlgorithm;
using Semight.Fwm.Algorithm.WaveLengthAlgorithm.Model.Enum;
using Semight.Fwm.Common.CommonModels.Classes.Param;
using Semight.Fwm.Common.CommonModels.Classes.WaveLength;
using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.CustomUserControl.WaveLengthParam.Tools;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Function;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Semight.Fwm.Fwm8612Helper.CommonBusiness.WaveLengthCalc
{
    public static class WaveLengthCalculator
    {
        private static AlgoVersion algoVersion = AlgoVersion.BadPixel;

        private static LCParamBulider ParamBulider => GlobalVariable.LCParamBulider;

        /// <summary>
        /// 计算功率和波长
        /// </summary>
        /// <param name="compensate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static WaveLengthValue CalcWaveLengthAndPower(int[] cavity1, int[] cavity2, int[] powerArray, (bool broadCompensate, bool narrowCompensate) compensate)
        {
            try
            {
                var waveLengthValue = CalcWaveLength(cavity1, cavity2, compensate);
                waveLengthValue.Power = CalcPower(powerArray);

                return waveLengthValue;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///计算功率
        /// </summary>
        public static double CalcPower(int[] powerArray)
        {
            try
            {
                double power = powerArray[3];
                var param = ParamBulider.GetPowerParam();
                power = param != null ? power * param.Power_K + param.Power_B : power;

                return power;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 计算波长
        /// </summary>
        /// <param name="compensate"></param>
        /// <returns></returns>
        public static WaveLengthValue CalcWaveLength(int[] cavity1, int[] cavity2, (bool broadCompensate, bool narrowCompensate) compensate)
        {
            try
            {
                if (cavity1.Any(da => da >= 65535) || cavity2.Any(da => da >= 65535))
                    return new WaveLengthValue { OverExposed = true };

                var waveBand = RangeChecked(cavity1);
                var thinFizeau = ParamBulider.GetFizeauParams(waveBand, ThickNessType.Thin);
                var thickFizeau = ParamBulider.GetFizeauParams(waveBand, ThickNessType.Thick);

                var compensationParam = new List<CompensationParam>();
                if (compensate.Item1)
                    compensationParam.Add(ParamBulider.GetDispersionCompensation(waveBand, CavityType.Broad));
                if (compensate.Item2)
                    compensationParam.Add(ParamBulider.GetDispersionCompensation(waveBand, CavityType.Narrow));

                var value = AlgoHelper.CalcThickWaveLength(cavity1, cavity2, thinFizeau, thickFizeau, compensationParam, algoVersion);
                return value;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 验证波段
        /// </summary>
        /// <param name="thinIntensity"></param>
        /// <returns></returns>
        private static WaveBand RangeChecked(int[] thinIntensity)
        {
            var param = ParamBulider.GetFizeauParams(WaveBand.C, ThickNessType.Thin);
            var value = AlgoHelper.CalcThinWaveLength(thinIntensity, param);

            var waveband = WaveBand.None;
            var wavelength = value.Step1;

            if (wavelength > 0 && wavelength < 1330.0)
                waveband = WaveBand.O;
            else if (wavelength >= 1330.0 && wavelength < 1420.0)
                waveband = WaveBand.E;
            else if (wavelength >= 1420.0 && wavelength < 1515.0)
                waveband = WaveBand.S;
            else if (wavelength >= 1515.0 && wavelength < 1575.0)
                waveband = WaveBand.C;
            else if (wavelength >= 1575.0)
                waveband = WaveBand.L;

            if (waveband == WaveBand.None)
                throw new Exception($"验证当前波段错误，计算波长值为{wavelength}");
            return waveband;
        }
    }
}
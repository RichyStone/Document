using Semight.Fwm.Algorithm.WaveLengthAlgorithm.Implement;
using Semight.Fwm.Algorithm.WaveLengthAlgorithm.Model.AlgoParam;
using Semight.Fwm.Algorithm.WaveLengthAlgorithm.Model.Enum;
using Semight.Fwm.Common.CommonModels.Classes.Param;
using Semight.Fwm.Common.CommonModels.Classes.WaveLength;
using Semight.Fwm.Common.CommonModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Semight.Fwm.Algorithm.WaveLengthAlgorithm
{
    public static class AlgoHelper
    {
        /// <summary>
        /// 薄腔算法
        /// </summary>
        /// <param name="thinIntensity"></param>
        /// <param name="results"></param>
        /// <param name="thinParam"></param>
        /// <param name="algoVersion"></param>
        private static void AlgoThinResult(ref double[] thinIntensity, ref double[] results, FizeauParam thinParam, AlgoVersion algoVersion = AlgoVersion.BadPixel)
        {
            try
            {
                var thinWedge = ConvertToWedge(thinParam);

                switch (algoVersion)
                {
                    case
                        AlgoVersion.LocalFile:
                        LocalFileAlgo.GetThinCavityResult(ref thinIntensity, ref results, thinWedge);
                        break;

                    case AlgoVersion.Huawei:
                        HuaWeiAlgo.GetThinCavityResult(ref thinIntensity, ref results, thinWedge, 0);
                        break;

                    case AlgoVersion.BadPixel:
                        var algoParam = new BadPixelParam() { Badpixels_n = 0, Badpixels_p = Array.Empty<int>(), Period_Zero = 0 };
                        BadPixelAlgo.GetThinCavityResult(ref thinIntensity, ref results, thinWedge, algoParam);
                        break;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 厚腔算法
        /// </summary>
        /// <param name="thickIntensity"></param>
        /// <param name="results"></param>
        /// <param name="thickParam"></param>
        /// <param name="algoVersion"></param>
        private static void AlgoThickResult(ref double[] thickIntensity, ref double[] results, FizeauParam thickParam, AlgoVersion algoVersion = AlgoVersion.BadPixel, double lambda = 0)
        {
            try
            {
                var thickWedge = ConvertToWedge(thickParam);

                switch (algoVersion)
                {
                    case AlgoVersion.LocalFile:
                        LocalFileAlgo.GetThickCavityResult(ref thickIntensity, ref results, thickWedge);
                        break;

                    case AlgoVersion.Huawei:
                        HuaWeiAlgo.GetThickCavityResult(ref thickIntensity, ref results, thickWedge, 0);
                        break;

                    case AlgoVersion.BadPixel:
                        var algoParam = new BadPixelParam() { Badpixels_n = 0, Badpixels_p = Array.Empty<int>(), Period_Zero = 0 };
                        var lam = lambda == 0 ? results[2] : lambda;
                        BadPixelAlgo.GetThickCavityResult(ref thickIntensity, ref results, thickWedge, algoParam, lam);
                        break;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 菲索参数转换为算法用数据结构
        /// </summary>
        /// <param name="waveBand"></param>
        /// <param name="widthType"></param>
        private static Wedge ConvertToWedge(FizeauParam param)
        {
            var wedge = new Wedge()
            {
                elements = (int)param.TotalPixels,
                pixelsize = param.PixelWidth,
                wedge_spacing = param.ThicknessCavity,
                wedge_angle = param.AngleCavity,
                Zero_piont = param.RefCavity
            };

            return wedge;
        }

        /// <summary>
        /// 计算薄腔波长值
        /// </summary>
        /// <param name="thinIntensity"></param>
        /// <param name="thinParam"></param>
        /// <param name="compensationParam"></param>
        /// <param name="algoVersion"></param>
        /// <returns></returns>
        public static WaveLengthValue CalcThinWaveLength(int[] thinIntensity, FizeauParam thinParam, CompensationParam compensationParam = null, AlgoVersion algoVersion = AlgoVersion.BadPixel)
        {
            try
            {
                if (thinIntensity.Any(intensity => intensity >= 65535))
                    return new WaveLengthValue { OverExposed = true };

                double[] thin = new double[512];
                Array.Copy(thinIntensity, thin, thinIntensity.Length);
                double[] results = new double[16];

                AlgoThinResult(ref thin, ref results, thinParam, algoVersion);
                results[2] = compensationParam != null ? CompensationCal(results[2], compensationParam) : results[2];
                var res = new WaveLengthValue { Step1 = results[1], Step2 = results[2], Final = results[2], Ref = results[10], Interference = results[5] };
                return res;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 计算厚腔波长值
        /// </summary>
        /// <param name="thinIntensity"></param>
        /// <param name="thickIntensity"></param>
        /// <param name="thinParam"></param>
        /// <param name="thickParam"></param>
        /// <param name="compensations"></param>
        /// <param name="algoVersion"></param>
        /// <returns></returns>
        public static WaveLengthValue CalcThickWaveLength(int[] thinIntensity, int[] thickIntensity, FizeauParam thinParam, FizeauParam thickParam, List<CompensationParam> compensations = null, AlgoVersion algoVersion = AlgoVersion.BadPixel)
        {
            try
            {
                if (thinIntensity.Any(intensity => intensity >= 65535) || thickIntensity.Any(intensity => intensity >= 65535))
                    return new WaveLengthValue { OverExposed = true };

                double[] thin = new double[512];
                double[] thick = new double[512];
                Array.Copy(thinIntensity, thin, thinIntensity.Length);
                Array.Copy(thickIntensity, thick, thickIntensity.Length);
                double[] results = new double[16];

                AlgoThinResult(ref thin, ref results, thinParam, algoVersion);
                if (compensations != null && compensations.Count > 0 && compensations.Any(c => c.CavityType == CavityType.Broad))
                    results[2] = CompensationCal(results[2], compensations.First(c => c.CavityType == CavityType.Broad));

                AlgoThickResult(ref thick, ref results, thickParam, algoVersion);
                if (compensations != null && compensations.Count > 0 && compensations.Any(c => c.CavityType == CavityType.Narrow))
                    results[6] = CompensationCal(results[6], compensations.First(c => c.CavityType == CavityType.Narrow));

                var res = new WaveLengthValue { Step1 = results[1], Step2 = results[2], Final = results[6], Ref = results[10], Interference = results[5] };
                return res;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 补偿计算
        /// </summary>
        /// <param name="intput"></param>
        /// <param name="compensation"></param>
        /// <returns></returns>
        private static double CompensationCal(double intput, CompensationParam compensation)
        {
            double res = compensation != null ? Math.Pow(intput, 2) * compensation.K2 + Math.Pow(intput, 1) * compensation.K1 + compensation.B + intput : intput;
            return res;
        }
    }
}
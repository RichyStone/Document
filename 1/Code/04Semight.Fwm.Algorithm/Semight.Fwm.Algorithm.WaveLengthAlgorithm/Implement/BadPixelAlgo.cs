using Semight.Fwm.Algorithm.WaveLengthAlgorithm.Model.AlgoParam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Semight.Fwm.Algorithm.WaveLengthAlgorithm.Implement
{
    public static class BadPixelAlgo
    {
        private const string AlgoFileAddressBadpixel = "Libs\\BadpixelAlgo\\Wavemeter2023B.dll";

        [DllImport(AlgoFileAddressBadpixel, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Thin_Fizeau_Cavity_2023Bv1(int elements, int Badpixels_n, int[] Badpixels_p, double pixelsize,
                                                        double Zero_piont, double wedge_spacing, double wedge_angle,
                                                        double Period_Zero, double[] Intensity, double[] F_results);

        [DllImport(AlgoFileAddressBadpixel, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Thick_Fizeau_Cavity_2023Bv1(int elements, int Badpixels_n, int[] Badpixels_p, double pixelsize,
                                                        double Zero_piont, double wedge_spacing, double wedge_angle,
                                                        double Period_Zero, double Lambda1, double[] Intensity, double[] F_results);

        /// <summary>
        /// 获取薄腔数据（错点版本）
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="f1_Results"></param>
        /// <returns></returns>
        public static bool GetThinCavityResult(ref double[] Intensity, ref double[] f1_Results, Wedge wedge, BadPixelParam badPixelParam)
        {
            try
            {
                if (!File.Exists(AlgoFileAddressBadpixel))
                    throw new Exception("调用算法错误，调用算法DLL不存在！");

                Thin_Fizeau_Cavity_2023Bv1(wedge.elements, badPixelParam.Badpixels_n, badPixelParam.Badpixels_p, wedge.pixelsize, wedge.Zero_piont, wedge.wedge_spacing, wedge.wedge_angle,
                   badPixelParam.Period_Zero, Intensity, f1_Results);

                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取厚腔数据（错点版本）
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="f2_Results"></param>
        /// <returns></returns>
        public static bool GetThickCavityResult(ref double[] Intensity, ref double[] f2_Results, Wedge wedge, BadPixelParam badPixelParam, double lambda)
        {
            try
            {
                if (!File.Exists(AlgoFileAddressBadpixel))
                    throw new Exception("调用算法错误，调用算法DLL不存在！");

                Thick_Fizeau_Cavity_2023Bv1(wedge.elements, badPixelParam.Badpixels_n, badPixelParam.Badpixels_p, wedge.pixelsize, wedge.Zero_piont, wedge.wedge_spacing, wedge.wedge_angle,
                   badPixelParam.Period_Zero, lambda, Intensity, f2_Results);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
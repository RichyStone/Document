using Semight.Fwm.Algorithm.WaveLengthAlgorithm.Model.AlgoParam;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Semight.Fwm.Algorithm.WaveLengthAlgorithm.Implement
{
    public static class HuaWeiAlgo
    {
        private const string AlgoFileAddressHuaWei = "Libs\\HuaWeiAlgo\\wavemeter20220726.dll";

        [DllImport(AlgoFileAddressHuaWei, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Thin_Fizeau_Cavity_v20220726(double[] Intensity, double[] F_results,
            int elements, double pixelsize, double Zero_piont, double wedge_spacing, double wedge_angle, double Period_Zero);

        [DllImport(AlgoFileAddressHuaWei, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Thick_Fizeau_Cavity_v20220726(double[] Intensity, double[] F_results,
            int elements, double pixelsize, double Zero_piont, double wedge_spacing, double wedge_angle, double Period_Zero);

        /// <summary>
        /// 获取薄腔数据（华为版本）
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="f1_Results"></param>
        /// <returns></returns>
        public static bool GetThinCavityResult(ref double[] Intensity, ref double[] f1_Results, Wedge wedge, double Period_Zero)
        {
            try
            {
                if (!File.Exists(AlgoFileAddressHuaWei))
                    throw new Exception("调用算法错误，调用算法DLL不存在！");

                Thin_Fizeau_Cavity_v20220726(Intensity, f1_Results, wedge.elements, wedge.pixelsize, wedge.Zero_piont, wedge.wedge_spacing, wedge.wedge_angle, Period_Zero);

                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取厚腔数据（华为版本）
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="f2_Results"></param>
        /// <returns></returns>
        public static bool GetThickCavityResult(ref double[] Intensity, ref double[] f2_Results, Wedge wedge, double Period_Zero)
        {
            try
            {
                if (!File.Exists(AlgoFileAddressHuaWei))
                    throw new Exception("调用算法错误，调用算法DLL不存在！");

                Thick_Fizeau_Cavity_v20220726(Intensity, f2_Results, wedge.elements, wedge.pixelsize, wedge.Zero_piont, wedge.wedge_spacing, wedge.wedge_angle, Period_Zero);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
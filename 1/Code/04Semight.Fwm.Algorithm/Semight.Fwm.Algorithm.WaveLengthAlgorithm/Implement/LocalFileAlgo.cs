using Semight.Fwm.Algorithm.WaveLengthAlgorithm.Model.AlgoParam;
using Semight.Fwm.Algorithm.WaveLengthAlgorithm.Model.Enum;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Semight.Fwm.Algorithm.WaveLengthAlgorithm.Implement
{
    public static class LocalFileAlgo
    {
        private const string AlgoFileAddressOridinary = "Libs\\OridinaryAlgo\\wavemeter_toolsbox.dll";

        [DllImport(AlgoFileAddressOridinary, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Thin_Fizeau_Cavity_v1(double[] Intensity, double[] F_results);

        [DllImport(AlgoFileAddressOridinary, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Thick_Fizeau_Cavity(double[] Intensity, double[] F_results);

        /// <summary>
        /// 获取薄腔数据（基础版本）
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="f1_Results"></param>
        /// <returns></returns>
        public static bool GetThinCavityResult(ref double[] Intensity, ref double[] f1_Results, Wedge wedge)
        {
            try
            {
                if (!File.Exists(AlgoFileAddressOridinary))
                    throw new Exception("调用算法错误，调用算法DLL不存在！");

                SaveFizeauParam(CavityThickness.Thin, wedge);
                Thin_Fizeau_Cavity_v1(Intensity, f1_Results);
                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取厚腔数据（基础版本）
        /// </summary>
        /// <param name="Intensity"></param>
        /// <param name="f2_Results"></param>
        /// <returns></returns>
        public static bool GetThickCavityResult(ref double[] Intensity, ref double[] f2_Results, Wedge wedge)
        {
            try
            {
                if (!File.Exists(AlgoFileAddressOridinary))
                    throw new Exception("调用算法错误，调用算法DLL不存在！");

                SaveFizeauParam(CavityThickness.Thick, wedge);
                Thick_Fizeau_Cavity(Intensity, f2_Results);
                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 保存要使用的菲索参数
        /// </summary>
        /// <param name="wlWidthTYpe"></param>
        /// <param name="Fizeau"></param>
        private static void SaveFizeauParam(CavityThickness cavityThickness, Wedge Fizeau)
        {
            try
            {
                byte[] buffer = StructToByteArray(Fizeau);
                var path = cavityThickness == CavityThickness.Thin ? FizeaufilepathThin : FizeaufilepathThick;
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(buffer);
                bw.Close();
            }
            catch
            {
                throw;
            }
        }

        private static string FizeaufilepathThin = System.Environment.CurrentDirectory + "\\Fizeau_sys0.dat";
        private static string FizeaufilepathThick = System.Environment.CurrentDirectory + "\\Fizeau_sys.dat";

        private static byte[] StructToByteArray(Wedge anytype)
        {
            try
            {
                // This function copies the structure data into a byte[]

                //Set the buffer to the correct size
                byte[] buffer = new byte[Marshal.SizeOf(anytype)];

                //Allocate the buffer to memory and pin it so that GC cannot use the
                //space (Disable GC)
                GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                // copy the struct into int byte[] mem alloc
                Marshal.StructureToPtr(anytype, h.AddrOfPinnedObject(), false);

                h.Free(); //Allow GC to do its job

                return buffer; // return the byte[]. After all that's why we are here

                // right.
            }
            catch
            {
                throw;
            }
        }
    }
}
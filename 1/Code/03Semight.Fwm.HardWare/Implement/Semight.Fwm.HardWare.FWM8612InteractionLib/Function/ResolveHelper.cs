using System;
using System.Collections.Generic;
using System.Text;

namespace Semight.Fwm.HardWare.FWM8612InteractionLib.Function
{
    public static class ResolveHelper
    {  /// <summary>
       /// 解析CCD数据
       /// </summary>
       /// <param name="bytes"></param>
       /// <returns></returns>
        public static int[] ResolveCCDData(byte[] bytes)
        {
            var temp = new int[512];
            for (int i = 0; i < 512; i++)
                temp[i] = (bytes[i * 2 + 1] << 8) + bytes[i * 2];

            bytes = null;
            return temp;
        }

        public static int ResolveHardWareData(byte[] bytes)
        {
            var temp = new string[4];
            for (int i = 0; i < 4; i++)
                temp[i] = bytes[i].ToString("x2");

            var value = temp[3] + temp[2] + temp[1] + temp[0];

            int result = Convert.ToInt32(value, 16);

            bytes = null;
            return result;
        }
    }
}
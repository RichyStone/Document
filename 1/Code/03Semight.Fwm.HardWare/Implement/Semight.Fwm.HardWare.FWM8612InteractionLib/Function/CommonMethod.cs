using System;
using System.Collections.Generic;
using System.Text;

namespace Semight.Fwm.HardWare.FWM8612InteractionLib.Function
{
    public static class Fwm8612CommonMethod
    {
        public static int ConvertToInt(string str)
        {
            if (int.TryParse(str, out var val))
                return val;
            else
                return -1;
        }

        public static double ConvertToDouble(string str)
        {
            if (double.TryParse(str, out var val))
                return val;
            else
                return -1;
        }
    }
}
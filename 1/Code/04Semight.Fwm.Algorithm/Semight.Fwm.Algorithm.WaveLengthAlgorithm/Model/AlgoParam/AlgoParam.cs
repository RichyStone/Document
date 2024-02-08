using System;
using System.Collections.Generic;
using System.Text;

namespace Semight.Fwm.Algorithm.WaveLengthAlgorithm.Model.AlgoParam
{
    public class BadPixelParam
    {
        public double Period_Zero { get; set; }

        public int Badpixels_n { get; set; }

        public int[] Badpixels_p { get; set; }
    }

    public struct Wedge
    {
        public int elements;
        public double pixelsize;
        public double Zero_piont;
        public double wedge_spacing;
        public double wedge_angle;
    };
}
using Semight.Fwm.Common.CommonModels.Enums;
using System;

namespace Semight.Fwm.Common.CommonModels.Classes.Param
{
    /// <summary>
    /// 菲索参数
    /// </summary>
    [Serializable]
    public class FizeauParam
    {
        public WaveBand WaveBand { get; set; }

        public ThickNessType Thickness { get; set; }

        public double TotalPixels { get; set; } = 512;

        public double PixelWidth { get; set; } = 25000;

        public double ThicknessCavity { get; set; }

        public double AngleCavity { get; set; }

        public double RefCavity { get; set; } = 250;

        public FizeauParam Copy()
        {
            var copy = new FizeauParam
            {
                WaveBand = WaveBand,
                AngleCavity = AngleCavity,
                PixelWidth = PixelWidth,
                RefCavity = RefCavity,
                Thickness = Thickness,
                ThicknessCavity = ThicknessCavity,
                TotalPixels = TotalPixels,
            };
            return copy;
        }

        public FizeauParam Copy(WaveBand waveBand)
        {
            var copy = new FizeauParam
            {
                WaveBand = waveBand,
                AngleCavity = AngleCavity,
                PixelWidth = PixelWidth,
                RefCavity = RefCavity,
                Thickness = Thickness,
                ThicknessCavity = ThicknessCavity,
                TotalPixels = TotalPixels,
            };
            return copy;
        }
    }
}
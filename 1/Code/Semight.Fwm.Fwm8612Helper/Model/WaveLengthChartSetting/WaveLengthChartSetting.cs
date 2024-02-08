using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semight.Fwm.Fwm8612Helper.Model.WaveLengthChartSetting
{
    public class WaveLengthChartSetting
    {
        public bool WaveLengthVisible { get; set; } = true;

        public bool PowerVisible { get; set; } = true;

        public bool PointXAxis { get; set; } = true;

        public bool TimeXAxis { get; set; }

        public int RefreshInterval { get; set; } = 20;

        public int ViewRange { get; set; } = 5000;

        public string? DataSavePath { get; set; } = $"{Environment.CurrentDirectory}\\Data\\TempData.xlsx";
    }
}
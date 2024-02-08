using Semight.Fwm.Common.CommonModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semight.Fwm.Fwm8612Helper.Model.SystemSetting
{
    [Serializable]
    public class SoftwareSetting
    {
        public bool SaveDataToLocalAuto { get; set; }

        public bool LowerComputerCalculate { get; set; }

        public bool UpperComputerCalculate { get; set; } = true;

        public bool FitStatus { get; set; }
    }
}
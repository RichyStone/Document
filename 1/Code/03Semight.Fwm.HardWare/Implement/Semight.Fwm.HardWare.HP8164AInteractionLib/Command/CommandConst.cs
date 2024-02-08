using System;
using System.Collections.Generic;
using System.Text;

namespace Semight.Fwm.HardWare.HP8164AInteractionLib.Command
{
    public class CommandConst
    {
        public const string CMD_QueryWL = ":sour0:wavelength?";
        public const string CMD_SetWL = ":sour0:wavelength ";
        public const string CMD_SetPower = ":sour0:pow:level ";
        public const string CMD_ReadPower = ":sour0:pow:level?";
        public const string CMD_OutputState = ":sour0:pow:stat ";
        public const string CMD_QueryState = ":sour0:pow:stat?";
        public const string CMD_AutoCalState = ":sour0:wav:corr:aut ";
        public const string CMD_AutoAlign = ":sour0:wav:corr:ara:all";
        public const string CMD_LaserTemp = ":sour0:wav:corr:zero:temp:last";
    }
}
using Semight.Fwm.Common.CommonModels.Classes.Param;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonTools.Serialize;
using System;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.Tools
{
    public static class ParamLocalHandler
    {
        /// <summary>
        /// 本地参数路径
        /// </summary>
        public static string LowerComputerParamPath { get; set; } = $"{Environment.CurrentDirectory}\\Param";

        /// <summary>
        /// 读取本地参数
        /// </summary>
        public static LowerComputerParam? ReadParamFromLocal(string readName = "LowerComputerParam")
        {
            try
            {
                if (XmlSerializeHelper.ReadFile<LowerComputerParam>($"{LowerComputerParamPath}\\{readName}", out var param))
                    return param;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("读取本地下位机参数失败", ex);
                return null;
            }
        }

        /// <summary>
        /// 保存参数到本地
        /// </summary>
        public static bool SaveParamToLocal(LowerComputerParam lowerComputerParam, string saveName)
        {
            try
            {
                if (string.IsNullOrEmpty(saveName))
                    saveName = "LowerComputerParam";
                return XmlSerializeHelper.WriteFile(lowerComputerParam, $"{LowerComputerParamPath}\\{saveName.Trim()}");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("读取本地下位机参数失败", ex);
                return false;
            }
        }
    }
}
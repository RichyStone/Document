using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Semight.Fwm.Common.CommonTools.ConfigueAppSetting
{
    public class ConfigHelper
    {
        /// <summary>
        /// 获取对应键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static string GetAppSetting(string key, string defaultValue = "")
        {
            try
            {
                var str = ConfigurationManager.AppSettings[key];
                return string.IsNullOrEmpty(str) ? defaultValue : str;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取对应键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        public static int GetAppSettingToInt(string key, int defaultValue)
        {
            try
            {
                var str = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(str) && int.TryParse(str, out var val))
                    return val;
                else
                    return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取对应键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        public static double GetAppSettingToDouble(string key, double defaultValue)
        {
            try
            {
                var str = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(str) && double.TryParse(str, out var val))
                    return val;
                else
                    return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取对应键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        public static bool GetAppSettingToBool(string key, bool defaultValue = false)
        {
            try
            {
                var str = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(str) && bool.TryParse(str, out var val))
                    return val;
                else
                    return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取对应键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        public static float GetAppSettingToFloat(string key, float defaultValue)
        {
            try
            {
                var str = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(str) && float.TryParse(str, out var val))
                    return val;
                else
                    return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 设定AppSetting
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>是否成功</returns>
        public static bool SetAppSetting(string key, string value, out string errMsg)
        {
            try
            {
                errMsg = string.Empty;
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (config == null)
                {
                    errMsg = "打开config文件失败！";
                    return false;
                }

                var appsettings = (AppSettingsSection)config.GetSection("appSettings");
                if (appsettings.Settings.AllKeys.Contains(key))
                    appsettings.Settings[key].Value = value;
                else
                    appsettings.Settings.Add(key, value);

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 设定AppSetting
        /// </summary>
        /// <param name="keyValuePairs">键值对字典</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>是否成功</returns>
        public static bool SetAppSetting(Dictionary<string, string> keyValuePairs, out string errMsg)
        {
            try
            {
                errMsg = string.Empty;
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (config == null)
                {
                    errMsg = "打开config文件失败！";
                    return false;
                }

                var appsettings = (AppSettingsSection)config.GetSection("appSettings");
                foreach (var pair in keyValuePairs)
                {
                    if (string.IsNullOrEmpty(pair.Key)) continue;

                    if (appsettings.Settings.AllKeys.Contains(pair.Key))
                        appsettings.Settings[pair.Key].Value = pair.Value;
                    else
                        appsettings.Settings.Add(pair.Key, pair.Value);
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                return true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }
    }
}
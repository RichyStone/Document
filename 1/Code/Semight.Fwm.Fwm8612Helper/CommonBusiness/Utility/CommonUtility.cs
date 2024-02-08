using Microsoft.Win32;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonTools.Serialize;
using Semight.Fwm.Common.CommonUILib.CustomUserControl.LogView;
using Semight.Fwm.Connection.ConnectionAssistantLib.USB;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.Model.SystemSetting;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using System;
using System.IO;
using System.Management;

namespace Semight.Fwm.Fwm8612Helper.CommonBusiness.Utility
{
    public static class CommonUtility
    {
        #region USB监控

        /// <summary>
        /// USB插拔检测
        /// </summary>
        private static readonly USBObserver usbCheck = new();

        /// <summary>
        /// 注册USB监控事件
        /// </summary>
        public static void RegisterUSBWatcher()
        {
            usbCheck.AddUSBInsertWatcher(USBEventHandler, TimeSpan.FromSeconds(1));
            usbCheck.AddUSBRemoveWatcher(USBEventHandler, TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// 注销USB监控事件
        /// </summary>
        public static void UnRegisterUSBWatcher()
        {
            usbCheck.RemoveUSBInsertWatcher();
            usbCheck.RemoveUSBRemoveWatcher();
        }

        /// <summary>
        /// USB插拔事件Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void USBEventHandler(Object sender, EventArrivedEventArgs e)
        {
            var watcher = sender as ManagementEventWatcher;
            watcher?.Stop();

            var device = USBObserver.GetUSBControllerDevice(e);
            if (device == null) return;

            if (e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent")
            {
                LogHelper.LogInfo($"检测到有USB连接：{device.Dependent}");
                ViewLogger.Info($"检测到有USB连接：{device.Dependent}");
            }
            else if (e.NewEvent.ClassPath.ClassName == "__InstanceDeletionEvent")
            {
                LogHelper.LogInfo($"检测到USB掉线：{device.Dependent}");
                ViewLogger.Info($"检测到USB掉线：{device.Dependent}");
            }
            watcher?.Start();

            foreach (var item in USBObserver.GetPortDeviceName())
                ViewLogger.Info($"{item}");
        }

        #endregion USB监控

        #region 单位转换

        /// <summary>
        /// 根据单位获取波长
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="wavelength"></param>
        /// <returns></returns>
        public static double GetWavelengthByUnit(WaveLengthUnit unit, double wavelength)
        {
            try
            {
                double wl = wavelength;
                switch (unit)
                {
                    case WaveLengthUnit.nm:
                        break;

                    case WaveLengthUnit.THz:
                        wl = (299.792458 / wavelength * 1000);
                        break;

                    case WaveLengthUnit.Cm_1:
                        wl = 1 / (wavelength * 1E-7);
                        break;
                }
                return wl;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 根据单位获取功率
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public static double GetPowerByUnit(PowerUnit unit, double power)
        {
            double res = power;
            if (unit.Equals(PowerUnit.mW))
                res = Math.Pow(10.0, (power / 10));

            return res;
        }

        #endregion 单位转换

        #region FileDialog

        /// <summary>
        /// 获取让用户选择保存文件的绝对路径
        /// </summary>
        /// <returns></returns>
        public static string GetSaveFileRoute(string filter, string fileName, string initialDir = "")
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = filter;
            dialog.RestoreDirectory = true;
            if (string.IsNullOrEmpty(initialDir))
                dialog.InitialDirectory = $"{Environment.CurrentDirectory}";
            dialog.OverwritePrompt = true;
            dialog.FileName = fileName;

            if (dialog.ShowDialog() == true)
                return dialog.FileName;

            return "";
        }

        #endregion FileDialog

        #region SoftwareSetting

        public static void ReadSoftwareSetting()
        {
            var path = $"{Environment.CurrentDirectory}\\SystemSetting\\setting.xml";
            if (File.Exists(path) && XmlSerializeHelper.ReadFile(path, out SoftwareSetting setting))
                GlobalConfig.SoftwareSetting = setting;
        }

        public static void WriteSoftwareSetting()
        {
            var dirPath = $"{Environment.CurrentDirectory}\\SystemSetting";
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            XmlSerializeHelper.WriteFile(GlobalConfig.SoftwareSetting, $"{dirPath}\\setting");
        }

        #endregion SoftwareSetting
    }
}
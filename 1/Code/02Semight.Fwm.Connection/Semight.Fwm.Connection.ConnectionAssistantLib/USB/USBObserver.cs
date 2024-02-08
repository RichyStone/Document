using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using LibUsbDotNet.Main;
using System.Linq;

namespace Semight.Fwm.Connection.ConnectionAssistantLib.USB
{
    /// <summary>
    /// USB控制设备类型
    /// </summary>
    public class USBControllerDevice
    {
        /// <summary>
        /// USB控制器设备ID
        /// </summary>
        public string Antecedent;

        /// <summary>
        /// USB即插即用设备ID
        /// </summary>
        public string Dependent;
    }

    public class USBObserver
    {
        /// <summary>
        /// USB插入事件监视
        /// </summary>
        private ManagementEventWatcher insertWatcher = null;

        /// <summary>
        /// USB拔出事件监视
        /// </summary>
        private ManagementEventWatcher removeWatcher = null;

        /// <summary>
        /// USB插入监视
        /// </summary>
        /// <param name="usbInsertHandler"></param>
        /// <param name="insertInterval"></param>
        /// <returns></returns>
        public bool AddUSBInsertWatcher(EventArrivedEventHandler usbInsertHandler, TimeSpan insertInterval)
        {
            try
            {
                ManagementScope Scope = new ManagementScope("root\\CIMV2");
                Scope.Options.EnablePrivileges = true;

                // USB插入监视
                if (usbInsertHandler != null)
                {
                    WqlEventQuery InsertQuery = new WqlEventQuery("__InstanceCreationEvent",
                        insertInterval,
                        "TargetInstance isa 'Win32_USBControllerDevice'");

                    insertWatcher = new ManagementEventWatcher(Scope, InsertQuery);
                    insertWatcher.EventArrived += usbInsertHandler;
                    insertWatcher.Start();
                }

                return true;
            }
            catch (Exception)
            {
                RemoveUSBInsertWatcher();
                return false;
            }
        }

        /// <summary>
        /// USB拔出监视
        /// </summary>
        /// <param name="usbRemoveHandler"></param>
        /// <param name="removeInterval"></param>
        /// <returns></returns>
        public bool AddUSBRemoveWatcher(EventArrivedEventHandler usbRemoveHandler, TimeSpan removeInterval)
        {
            try
            {
                ManagementScope Scope = new ManagementScope("root\\CIMV2");
                Scope.Options.EnablePrivileges = true;

                // USB拔出监视
                if (usbRemoveHandler != null)
                {
                    WqlEventQuery RemoveQuery = new WqlEventQuery("__InstanceDeletionEvent",
                        removeInterval,
                        "TargetInstance isa 'Win32_USBControllerDevice'");

                    removeWatcher = new ManagementEventWatcher(Scope, RemoveQuery);
                    removeWatcher.EventArrived += usbRemoveHandler;
                    removeWatcher.Start();
                }

                return true;
            }
            catch (Exception)
            {
                RemoveUSBRemoveWatcher();
                return false;
            }
        }

        /// <summary>
        /// 移去USB事件监视器
        /// </summary>
        public void RemoveUSBInsertWatcher()
        {
            if (insertWatcher != null)
            {
                insertWatcher.Stop();
                insertWatcher = null;
            }
        }

        /// <summary>
        /// 移去USB事件监视器
        /// </summary>
        public void RemoveUSBRemoveWatcher()
        {
            if (removeWatcher != null)
            {
                removeWatcher.Stop();
                removeWatcher = null;
            }
        }

        /// <summary>
        /// 定位发生插拔的USB设备
        /// </summary>
        /// <param name="e">USB插拔事件参数</param>
        /// <returns>发生插拔现象的USB控制设备ID</returns>
        public static USBControllerDevice GetUSBControllerDevice(EventArrivedEventArgs e)
        {
            if (e.NewEvent["TargetInstance"] is ManagementBaseObject mbo && mbo.ClassPath.ClassName == "Win32_USBControllerDevice")
            {
                var Antecedent = (mbo["Antecedent"] as string).Replace("\"", string.Empty).Split('=')[1];
                var Dependent = (mbo["Dependent"] as string).Replace("\"", string.Empty).Split('=')[1];
                return new USBControllerDevice { Antecedent = Antecedent, Dependent = Dependent };
            }

            return null;
        }

        public static List<string> GetPortDeviceName()
        {
            var result = new List<string>();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher
            ("select * from Win32_PnPEntity where Name like '%(COM%'"))
            {
                var hardInfos = searcher.Get();
                foreach (var hardInfo in hardInfos)
                {
                    if (hardInfo.Properties["Name"].Value != null)
                    {
                        string deviceName = hardInfo.Properties["Name"].Value.ToString();
                        int startIndex = deviceName.IndexOf("(");
                        int endIndex = deviceName.IndexOf(")");
                        if (startIndex != -1 && endIndex != -1)
                        {
                            string key = deviceName.Substring(startIndex + 1, deviceName.Length - startIndex - 2);
                            string name = deviceName.Substring(0, startIndex - 1);
                            result.Add("key:" + key + ",name:" + name + ",deviceName:" + deviceName);
                        }
                    }
                }
            }

            return result;
        }
    }
}
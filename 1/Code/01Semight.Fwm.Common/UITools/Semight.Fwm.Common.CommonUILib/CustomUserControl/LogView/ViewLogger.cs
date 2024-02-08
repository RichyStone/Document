using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Semight.Fwm.Common.CommonUILib.CustomUserControl.LogView
{
    public static class ViewLogger
    {
        private static UCLogViewer? visualControl;

        public static void Register(UCLogViewer uCLogViewer)
        {
            visualControl = uCLogViewer;
        }

        public static void Info(string msg)
        {
            visualControl?.AddLog(msg, LoggerType.Info);
        }

        public static void Warning(string msg)
        {
            visualControl?.AddLog(msg, LoggerType.Warning);
        }

        public static void Error(string msg)
        {
            visualControl?.AddLog(msg, LoggerType.Error);
        }

        public static void Success(string msg)
        {
            visualControl?.AddLog(msg, LoggerType.Success);
        }
    }
}
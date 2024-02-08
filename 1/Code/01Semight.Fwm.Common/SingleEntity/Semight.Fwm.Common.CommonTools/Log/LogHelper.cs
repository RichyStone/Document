using System;
using System.IO;
using System.Linq;

namespace Semight.Fwm.Common.CommonTools.Log
{
    public static class LogHelper
    {
        /// <summary>
        /// 信息Logger
        /// </summary>
        private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");

        /// <summary>
        /// 错误Logger
        /// </summary>
        private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");

        /// <summary>
        /// 初始化标志位
        /// </summary>
        private static bool initFlag = false;

        /// <summary>
        /// 初始化锁
        /// </summary>
        private static readonly object initLock = new object();

        /// <summary>
        /// 初始化
        /// </summary>
        private static void LogInit()
        {
            lock (initLock)
            {
                if (!initFlag)
                    initFlag = LoadLogConfig();
            }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        private static bool LoadLogConfig()
        {
            try
            {
                var directories = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}", "*.config", SearchOption.AllDirectories);
                var path = directories.FirstOrDefault(d => d.Contains("log4net"));
                if (string.IsNullOrEmpty(path))
                    return false;

                var collection = log4net.Config.XmlConfigurator.Configure(new FileInfo($"{path}"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 普通信息
        /// </summary>
        /// <param name="info"></param>
        public static void LogInfo(string message)
        {
            if (!initFlag)
                LogInit();

            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(message);
            }
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="info"></param>
        public static void LogDebug(string message)
        {
            if (!initFlag)
                LogInit();

            if (loginfo.IsDebugEnabled)
            {
                loginfo.Debug(message);
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="error"></param>
        public static void LogError(string message)
        {
            if (!initFlag)
                LogInit();

            if (logerror.IsErrorEnabled)
            {
                logerror.Error(message);
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ex"></param>
        public static void LogError(string message, Exception ex)
        {
            if (!initFlag)
                LogInit();

            Console.WriteLine(message);
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(message, ex);
            }
        }
    }
}
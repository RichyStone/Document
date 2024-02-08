using Semight.Fwm.Common.CommonTools.Log;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace Semight.Fwm.Fwm8612Helper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? singletonMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            Process p = Process.GetCurrentProcess();
            singletonMutex = new Mutex(true, p.ProcessName, out bool createNew);
            if (!createNew)
            {
                MessageBox.Show("Application is already run!");
                this.Shutdown();
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogHelper.LogError("UI线程发生未处理错误", e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                LogHelper.LogError("非UI线程发生未处理错误", ex);
        }
    }
}
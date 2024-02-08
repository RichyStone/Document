#define SelfTest

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Measure;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.ViewManager;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using System;
using System.Threading.Tasks;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.WaveLengthMesure
{
    public partial class WaveLengthMeasureViewModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// 连接状态
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(MeasureStateChangeCommand))]
        private bool connected;

        /// <summary>
        /// 测量状态
        /// </summary>
        [ObservableProperty]
        private bool isMeasuring;

        /// <summary>
        /// 设备标签
        /// </summary>
        [ObservableProperty]
        private string? instrumentTag;

        #endregion Fields

        #region Event

        [RelayCommand]
        private void Loaded()
        {
            RegisterErrorCallBackHandler();
        }

        [RelayCommand]
        private void Unloaded()
        {
        }

        /// <summary>
        /// 连接下位机
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ConnectionChange()
        {
            try
            {
                bool negativeTag = false;
                var task = !Connected ?
                    Task.Run(() =>
                    {
                        if (GlobalConfig.ConnectionWay == ConnectionWay.USB)
                            return MeasureContext.Connect();
                        else
                            return MeasureContext.Connect(GlobalConfig.IP, GlobalConfig.Port);
                    }) :
                    Task.Run(() =>
                    {
                        negativeTag = true;
                        IsMeasuring = false;
                        return MeasureContext.DisConnect();
                    });

                await task;

                if (task.Result)
                {
                    Connected = !Connected;
                    WeakReferenceMessenger.Default.Send(new MessagerTransData<bool> { Value = Connected }, MessagerProtocal.ConnectState);
                }
                else
                {
                    var prefix = negativeTag ? "Disconnection" : "Connection";
                    MessageBoxHelper.ErrorBox($"{InstrumentTag} {prefix} Failed");
                }
            }
            catch (TimeoutException timeOutex)
            {
                MessageBoxHelper.ErrorBox($"连接超时：{timeOutex.Message}");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ErrorBox($"连接或断连出现报错：{ex.Message}");
                LogHelper.LogError("连接或断连出现报错", ex);
            }

            OnPropertyChanged(nameof(Connected));
        }

        /// <summary>
        /// 测量状态改变事件
        /// </summary>
#if SelfTest

        [RelayCommand]
#else

        [RelayCommand(CanExecute = nameof(Connected))]
#endif
        private async Task MeasureStateChange()
        {
            bool res;
            if (!IsMeasuring)
                res = await Task.Run(() => { return MeasureContext.StartMeasure(); });
            else
                res = MeasureContext.StopMeasure();

            if (res)
            {
                IsMeasuring = !IsMeasuring;
                WeakReferenceMessenger.Default.Send(new MessagerTransData<bool> { Value = IsMeasuring }, MessagerProtocal.MeasureState);
            }

            OnPropertyChanged(nameof(IsMeasuring));
        }

        /// <summary>
        /// 打开参数窗口
        /// </summary>
        [RelayCommand]
        private void OpenParamWindow()
        {
            ViewManager.ShowWindow(WindowType.LoginWindow, LoginCallBack: () => ViewManager.ShowWindow(WindowType.LCParamWindow));
        }

        [RelayCommand]
        private void OpenWaveLengthChartWindow()
        {
            ViewManager.ShowWindow(WindowType.WaveLengthChartWindow);
        }

        [RelayCommand]
        private void OpenIpSettingWindow()
        {
            ViewManager.ShowWindow(WindowType.WinIpSetting);
        }

        /// <summary>
        /// 打开系统设定窗口
        /// </summary>
        [RelayCommand]
        private void OpenSystemSettingWindow()
        {
            ViewManager.ShowWindow(WindowType.LoginWindow, LoginCallBack: () => ViewManager.ShowWindow(WindowType.WinSystemSetting));
        }

        #endregion Event

        /// <summary>
        /// 注册错误回调处理
        /// </summary>
        private void RegisterErrorCallBackHandler()
        {
            MeasureContext.ErrorAction -= StartMeasureErrorAction;
            MeasureContext.ErrorAction += StartMeasureErrorAction;
        }

        /// <summary>
        /// 测量开启失败回调
        /// </summary>
        private void StartMeasureErrorAction(string message)
        {
            IsMeasuring = false;
            MessageBoxHelper.ErrorBox($"{message}！");
        }
    }
}
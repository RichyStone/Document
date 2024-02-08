using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Semight.Fwm.Common.CommonModels.Classes.HardWare;
using Semight.Fwm.Common.CommonModels.Enums;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.SystemSetting
{
    public partial class IPSettingViewModel : ObservableObject
    {
        private FWM8612Context FwmContext => FWM8612Context.GetInstance();
        public bool Connected => FwmContext.Connected;

        [ObservableProperty]
        private IPSettingModel model = new();

        #region Event

        [RelayCommand]
        private void Load()
        {
            Model.IP = GlobalConfig.IP;
            Model.Port = GlobalConfig.Port.ToString();

            OnPropertyChanged(nameof(Model));
        }

        [RelayCommand]
        private void Unload()
        {
        }

        [RelayCommand]
        private void ChangeConnectWay(SelectionChangedEventArgs e)
        {
            var str = (e.Source as ComboBox)?.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(str) && str.ToLower().Contains("usb"))
                GlobalConfig.ConnectionWay = ConnectionWay.USB;
            else
                GlobalConfig.ConnectionWay = ConnectionWay.Net;
        }

        /// <summary>
        /// 读取设定
        /// </summary>
        [RelayCommand]
        private void Read()
        {
            if (!Connected)
                return;

            Model.IP = FwmContext.GetIPSetting(IpAddressType.Ip);
            Model.Port = FwmContext.GetIPSetting(IpAddressType.Port);
            Model.Mask = FwmContext.GetIPSetting(IpAddressType.Mask);
            Model.Gate = FwmContext.GetIPSetting(IpAddressType.Gate);

            OnPropertyChanged(nameof(Model));
        }

        /// <summary>
        /// 保存设定
        /// </summary>
        [RelayCommand]
        private void Save()
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.IP))
                    SetIP(Model.IP);

                if (!string.IsNullOrEmpty(Model.Gate))
                    SetGate(Model.Gate);

                if (!string.IsNullOrEmpty(Model.Mask))
                    SetMask(Model.Mask);

                if (!string.IsNullOrEmpty(Model.Port))
                    SetPort(Model.Port);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("保存IP设定失败", ex);
                MessageBoxHelper.ErrorBox("保存IP设定失败");
            }
        }

        #endregion Event

        #region Method

        /// <summary>
        /// 设置IP
        /// </summary>
        /// <param name="ip"></param>
        public void SetIP(string ip)
        {
            if (IPCheck(ip))
                FwmContext.SetIPSetting(IpAddressType.Ip, ip);
            else
                MessageBoxHelper.WarningBox("IP格式不正确！");
        }

        /// <summary>
        /// 设置掩码
        /// </summary>
        /// <param name="mask"></param>
        public void SetMask(string mask)
        {
            if (IPCheck(mask))
                FwmContext.SetIPSetting(IpAddressType.Mask, mask);
            else
                MessageBoxHelper.WarningBox("掩码格式不正确！");
        }

        /// <summary>
        /// 设置网关
        /// </summary>
        /// <param name="gate"></param>
        public void SetGate(string gate)
        {
            if (IPCheck(gate))
                FwmContext.SetIPSetting(IpAddressType.Gate, gate);
            else
                MessageBoxHelper.WarningBox("网关格式不正确！");
        }

        /// <summary>
        /// 设置端口号
        /// </summary>
        /// <param name="port"></param>
        public void SetPort(string port)
        {
            FwmContext.SetIPSetting(IpAddressType.Ip, port);
        }

        /// <summary>
        /// 检查IP格式
        /// IPAddress.TryParse("8", out ip)会得到“0.0.0.8”的输出,所以只能用正则来做
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool IPCheck(string IP)
        {
            return Regex.IsMatch(IP, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        #endregion Method
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Semight.Fwm.Common.CommonTools.ConfigueAppSetting;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.ViewManager;
using System;
using System.Windows.Input;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.Login
{
    public partial class LoginViewModel : ObservableObject
    {
        /// <summary>
        /// 加载事件
        /// </summary>
        [RelayCommand]
        private void Load()
        {
        }

        /// <summary>
        /// 卸载事件
        /// </summary>
        [RelayCommand]
        private void Unload()
        {
        }

        [RelayCommand]
        private void LoginFunc(string? passWord)
        {
            try
            {
                if (string.IsNullOrEmpty(passWord))
                {
                    MessageBoxHelper.WarningBox("请输入密码！");
                    return;
                }

                var settingPassWord = ConfigHelper.GetAppSetting("PassWord", "semight1508");
                if (passWord.Trim().Equals(settingPassWord.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    ViewManager.CloseWindow(WindowType.LoginWindow);
                }
                else
                {
                    MessageBoxHelper.WarningBox("密码错误");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("登录失败", ex);
            }
        }
    }
}
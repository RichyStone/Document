using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Semight.Fwm.Common.CommonTools.Serialize;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Utility;
using Semight.Fwm.Fwm8612Helper.Model.SystemSetting;
using System;
using System.IO;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.SystemSetting
{
    public partial class SystemSettingViewModel : ObservableObject
    {
        [RelayCommand]
        private void Loaded()
        {
            CommonUtility.ReadSoftwareSetting();
        }

        [RelayCommand]
        private void Unloaded()
        {
        }

        [RelayCommand]
        private void SaveSystemSetting()
        {
            CommonUtility.WriteSoftwareSetting();
        }
    }
}
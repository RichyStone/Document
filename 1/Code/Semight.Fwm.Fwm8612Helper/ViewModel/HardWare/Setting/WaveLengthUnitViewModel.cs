using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.Fwm8612Helper.CommonBusiness.Config;
using Semight.Fwm.Fwm8612Helper.CommonUIAssistant.MessagerTools;
using Semight.Fwm.HardWare.FWM8612InteractionLib.Models.Enums;
using Semight.Fwm.HardWare.HardwarePlatform.FWM8612;
using System;

namespace Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting
{
    public partial class WaveLengthUnitViewModel : ObservableObject
    {
        private FWM8612Context FwmContext => FWM8612Context.GetInstance();

        public bool Connected => FWM8612Context.GetInstance().Connected;

        public bool InitialFlag { get; private set; }

        [RelayCommand]
        private void Loaded()
        {
            RegisterMessager();
        }

        [RelayCommand]
        private void Unloaded()
        {
        }

        /// <summary>
        /// 注册Messager
        /// </summary>
        private void RegisterMessager()
        {
            WeakReferenceMessenger.Default.Unregister<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState);
            WeakReferenceMessenger.Default.Register<MessagerTransData<bool>, string>(this, MessagerProtocal.ConnectState, ConnectionChangedHandler);

            WeakReferenceMessenger.Default.Unregister<MessagerTransData, string>(this, MessagerProtocal.RefreshHardWareSettings);
            WeakReferenceMessenger.Default.Register<MessagerTransData, string>(this, MessagerProtocal.RefreshHardWareSettings, RefreshHandler);
        }

        /// <summary>
        /// 连接状态改变回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void ConnectionChangedHandler(object sender, MessagerTransData<bool> transData)
        {
            if (transData.Value)
                InitialWaveLengthUnit();
            else
                InitialFlag = false;
        }

        /// <summary>
        /// 刷新设定回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="transData"></param>
        private void RefreshHandler(object sender, MessagerTransData transData)
        {
            if (Connected)
            {
                InitialFlag = false;
                InitialWaveLengthUnit();
            }
        }

        [ObservableProperty]
        private bool nmChecked;

        partial void OnNmCheckedChanged(bool value)
        {
            if (!value || !InitialFlag) return;
            SetWaveLengthUnit(WaveLengthUnit.nm);
        }

        [ObservableProperty]
        private bool tHzChecked;

        partial void OnTHzCheckedChanged(bool value)
        {
            if (!value || !InitialFlag) return;
            SetWaveLengthUnit(WaveLengthUnit.THz);
        }

        [ObservableProperty]
        private bool cmChecked;

        partial void OnCmCheckedChanged(bool value)
        {
            if (!value || !InitialFlag) return;
            SetWaveLengthUnit(WaveLengthUnit.Cm_1);
        }

        /// <summary>
        /// 初始化波长单位
        /// </summary>
        private void InitialWaveLengthUnit()
        {
            try
            {
                var unit = FwmContext.GetWaveLengthUnit();
                if (unit == WaveLengthUnit.nm)
                    NmChecked = true;
                else if (unit == WaveLengthUnit.THz)
                    THzChecked = true;
                else if (unit == WaveLengthUnit.Cm_1)
                    CmChecked = true;

                InitialFlag = true;
                GlobalConfig.WaveLengthUnit = unit;
                WeakReferenceMessenger.Default.Send(new MessagerTransData<WaveLengthUnit> { Value = unit }, MessagerProtocal.WavelengthUnitChanged);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("读取设定发生错误", ex);
            }
        }

        /// <summary>
        /// 设置波长单位
        /// </summary>
        /// <param name="waveLengthUnit"></param>
        private void SetWaveLengthUnit(WaveLengthUnit waveLengthUnit)
        {
            if (!Connected) return;

            try
            {
                FwmContext.SetWavelengthUnit(waveLengthUnit);
                GlobalConfig.WaveLengthUnit = waveLengthUnit;
                WeakReferenceMessenger.Default.Send(new MessagerTransData<WaveLengthUnit> { Value = waveLengthUnit }, MessagerProtocal.WavelengthUnitChanged);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }
    }
}
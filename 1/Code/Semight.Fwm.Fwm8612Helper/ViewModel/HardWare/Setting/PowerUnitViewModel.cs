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
    public partial class PowerUnitViewModel : ObservableObject
    {
        private FWM8612Context FwmContext => FWM8612Context.GetInstance();

        public bool Connected => FwmContext.Connected;

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
                InitialPowerUnit();
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
                InitialPowerUnit();
            }
        }

        [ObservableProperty]
        private bool dbmChecked;

        partial void OnDbmCheckedChanged(bool value)
        {
            if (!value || !InitialFlag) return;
            SetPowerUnit(PowerUnit.dBm);
        }

        [ObservableProperty]
        private bool mWChecked;

        partial void OnMWCheckedChanged(bool value)
        {
            if (!value || !InitialFlag) return;
            SetPowerUnit(PowerUnit.mW);
        }

        /// <summary>
        /// 初始化功率单位
        /// </summary>
        private void InitialPowerUnit()
        {
            try
            {
                var unit = FwmContext.GetPowerUnit();
                if (unit == PowerUnit.dBm)
                    DbmChecked = true;
                else if (unit == PowerUnit.mW)
                    MWChecked = true;

                InitialFlag = true;
                GlobalConfig.PowerUnit = unit;
                WeakReferenceMessenger.Default.Send(new MessagerTransData<PowerUnit> { Value = unit }, MessagerProtocal.PowerUnitChanged);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("读取设定发生错误", ex);
            }
        }

        /// <summary>
        /// 设置功率单位
        /// </summary>
        /// <param name="powerUnit"></param>
        private void SetPowerUnit(PowerUnit powerUnit)
        {
            if (!Connected) return;
            try
            {
                FwmContext.SetPowerUnit(powerUnit);
                GlobalConfig.PowerUnit = powerUnit;
                WeakReferenceMessenger.Default.Send(new MessagerTransData<PowerUnit> { Value = powerUnit }, MessagerProtocal.PowerUnitChanged);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入设定发生错误", ex);
                MessageBoxHelper.ErrorBox("写入设定发生错误！");
            }
        }
    }
}
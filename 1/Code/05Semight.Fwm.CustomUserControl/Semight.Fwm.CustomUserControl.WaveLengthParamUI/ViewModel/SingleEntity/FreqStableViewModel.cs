using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Semight.Fwm.Common.CommonModels.Classes.Param;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.CustomUserControl.WaveLengthParam.Tools;
using System;
using System.ComponentModel;
using System.Linq;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel.SingleEntity
{
    public partial class FreqStableViewModel : ObservableValidator
    {
        #region Fields

        [ObservableProperty]
        private BindingList<FreqStableParam>? bindingParams;

        #endregion Fields

        #region Method

        [RelayCommand]
        private void Loaded()
        {
            RefreshParam();
            RegisterMessager();
        }

        [RelayCommand]
        private void Unloaded()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }

        private void RegisterMessager()
        {
            WeakReferenceMessenger.Default.Register<MessagerTransData, string>(this, ParamMessagerProtocal.RefreshParam, RefreshParamFuction);
            WeakReferenceMessenger.Default.Register<MessagerTransData, string>(this, ParamMessagerProtocal.WriteParam, WriteParamFuction);
        }

        /// <summary>
        /// 刷新参数事件处理
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void RefreshParamFuction(object recipient, MessagerTransData message)
        {
            RefreshParam();
        }

        /// <summary>
        /// 写入参数回调处理
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void WriteParamFuction(object recipient, MessagerTransData message)
        {
            try
            {
                var param = GlobalVariable.LCParamBulider.CurrentParam?.FreqStableWLParam;
                if (param != null)
                    HardWareManager.WriteFreqStableParam(param);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入稳频光源参数失败", ex);
                MessageBoxHelper.ErrorBox("写入稳频光源参数失败");
            }
        }

        /// <summary>
        /// 刷新参数
        /// </summary>
        private void RefreshParam()
        {
            BindingParams = null;

            var param = GlobalVariable.LCParamBulider.CurrentParam?.FreqStableWLParam;
            if (param != null)
                BindingParams = new BindingList<FreqStableParam>(param.OrderBy(p => p.WaveBand).ToList());
        }

        #endregion Method
    }
}
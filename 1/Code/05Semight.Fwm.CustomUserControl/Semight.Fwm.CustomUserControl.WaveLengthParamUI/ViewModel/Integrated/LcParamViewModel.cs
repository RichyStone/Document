using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Semight.Fwm.Common.CommonTools.Log;
using Semight.Fwm.Common.CommonUILib.MessageBoxHelper;
using Semight.Fwm.Common.CommonUILib.MessagerTools;
using Semight.Fwm.CustomUserControl.WaveLengthParam.Model;
using Semight.Fwm.CustomUserControl.WaveLengthParam.Tools;
using Semight.Fwm.CustomUserControl.WaveLengthParam.View.SingleEntity;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel.Integrated
{
    public partial class LcParamViewModel : ObservableObject
    {
        #region Fields

        /// <summary>
        /// 内部控件
        /// </summary>
        [ObservableProperty]
        private ContentControl? innerControl;

        /// <summary>
        /// 保存名字
        /// </summary>
        [ObservableProperty]
        private string? saveName;

        /// <summary>
        /// 读取的名称集合
        /// </summary>
        public ObservableCollection<string> ReadNames { get; set; } = new ObservableCollection<string>();

        private IServiceProvider? _serviceProvider;

        #endregion Fields

        #region Method

        [RelayCommand]
        private void Loaded()
        {
            InitialControls();
            InnerControl = _serviceProvider?.GetService<UcFizeau>();

            InitialReadNames();
        }

        [RelayCommand]
        private void Unloaded()
        {
            InnerControl = null;
        }

        /// <summary>
        /// 修改内部控件
        /// </summary>
        /// <param name="key"></param>
        [RelayCommand]
        private void ChangeInnerControl(string key)
        {
            if (!Enum.TryParse(typeof(ControlType), key, out var res) || res == null || res is not ControlType controlType)
                return;

            switch (controlType)
            {
                case ControlType.Compensation:
                    InnerControl = _serviceProvider?.GetService<UcCompensation>();
                    break;

                case ControlType.MultiPeaks:
                    InnerControl = _serviceProvider?.GetService<UcMultiPeaks>();
                    break;

                case ControlType.Fizeau:
                    InnerControl = _serviceProvider?.GetService<UcFizeau>();
                    break;

                case ControlType.FreqStable:
                    InnerControl = _serviceProvider?.GetService<UcFreqStable>();
                    break;

                case ControlType.PowerParam:
                    InnerControl = _serviceProvider?.GetService<UcPowerParam>();
                    break;
            }
        }

        /// <summary>
        /// 读取参数
        /// </summary>
        [RelayCommand]
        private void Read()
        {
            if (!ReadParamFromLower())
                MessageBoxHelper.ErrorBox($"获取下位机参数失败！");
        }

        /// <summary>
        /// 写入参数
        /// </summary>
        [RelayCommand]
        private void Write()
        {
            try
            {
                WeakReferenceMessenger.Default.Send(new MessagerTransData(), ParamMessagerProtocal.WriteParam);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("写入参数失败", ex);
                MessageBoxHelper.ErrorBox("写入参数失败！");
            }
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        [RelayCommand]
        private void Save()
        {
            try
            {
                HardWareManager.SaveToLower();
            }
            catch (Exception ex)
            {
                LogHelper.LogError("保存参数失败", ex);
                MessageBoxHelper.ErrorBox("保存参数失败！");
            }
        }

        /// <summary>
        /// 从本地读取参数
        /// </summary>
        /// <param name="readName"></param>
        [RelayCommand]
        private void ReadFormLocal(string readName)
        {
            var param = ParamLocalHandler.ReadParamFromLocal(readName);
            if (param == null)
                MessageBoxHelper.ErrorBox($"读取本地参数失败！");
            else
            {
                GlobalVariable.LCParamBulider.SetLowerComputerParam(param);
                WeakReferenceMessenger.Default.Send(new MessagerTransData(), ParamMessagerProtocal.RefreshParam);
            }
        }

        /// <summary>
        /// 参数保存到本地
        /// </summary>
        /// <param name="saveName"></param>
        [RelayCommand]
        private void SaveToLocal(string saveName)
        {
            if (!ParamLocalHandler.SaveParamToLocal(GlobalVariable.LCParamBulider.CurrentParam, saveName))
                MessageBoxHelper.ErrorBox($"保存参数到本地失败！");
            else
            {
                InitialReadNames();
                MessageBoxHelper.SuccessBox($"保存成功！");
            }
        }

        /// <summary>
        /// 初始化本地参数列表
        /// </summary>
        private void InitialReadNames()
        {
            ReadNames.Clear();
            if (!Directory.Exists(ParamLocalHandler.LowerComputerParamPath)) return;

            var pathes = Directory.GetFiles(ParamLocalHandler.LowerComputerParamPath);
            var list = pathes.ToList().OrderByDescending(path => File.GetCreationTime(path));
            foreach (var item in list)
                ReadNames.Add(Path.GetFileNameWithoutExtension(item));
        }

        /// <summary>
        /// 读取参数
        /// </summary>
        /// <returns></returns>
        private bool ReadParamFromLower()
        {
            try
            {
                bool res;
                if (res = HardWareManager.ReadLowerCpParam())
                {
                    var param = HardWareManager.GetLowerComputerParam();

                    if (param != null)
                        GlobalVariable.LCParamBulider.SetLowerComputerParam(param);
                    WeakReferenceMessenger.Default.Send(new MessagerTransData(), ParamMessagerProtocal.RefreshParam);
                }

                return res;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("获取下位机参数失败", ex);
                return false;
            }
        }

        private void InitialControls()
        {
            var _serviceCollection = new ServiceCollection();

            _serviceCollection.AddTransient<UcCompensation>();
            _serviceCollection.AddTransient<UcFizeau>();
            _serviceCollection.AddTransient<UcFreqStable>();
            _serviceCollection.AddTransient<UcMultiPeaks>();
            _serviceCollection.AddTransient<UcPowerParam>();

            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }
    }

    #endregion Method
}
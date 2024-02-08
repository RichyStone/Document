using Semight.Fwm.Fwm8612Helper.ViewModel.Chart;
using Semight.Fwm.Fwm8612Helper.ViewModel.HardWare;
using Semight.Fwm.Fwm8612Helper.ViewModel.HardWare.Setting;
using Semight.Fwm.Fwm8612Helper.ViewModel.Login;
using Semight.Fwm.Fwm8612Helper.ViewModel.Param;
using Semight.Fwm.Fwm8612Helper.ViewModel.SystemSetting;
using Semight.Fwm.Fwm8612Helper.ViewModel.WaveLengthMesure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Semight.Fwm.Fwm8612Helper.ViewModel
{
    public class ViewModelLocator
    {
        private IServiceProvider? _serviceProvider;

        public ViewModelLocator()
        {
            var _serviceCollection = new ServiceCollection();

            _serviceCollection.AddTransient<MainViewModel>();

            ////Login
            _serviceCollection.AddTransient<LoginViewModel>();

            #region 下位机参数

            _serviceCollection.AddTransient<LcParamViewModel>();

            #endregion 下位机参数

            #region 硬件

            _serviceCollection.AddTransient<HardwareSettingViewModel>();

            #region 硬件设定子项

            _serviceCollection.AddTransient<AverageSettingViewModel>();
            _serviceCollection.AddTransient<CalibrationSettingViewModel>();
            _serviceCollection.AddTransient<ExposureSettingViewModel>();
            _serviceCollection.AddTransient<HardWareSnViewModel>();
            _serviceCollection.AddTransient<LaserModeViewModel>();
            _serviceCollection.AddTransient<LaserOperateViewModel>();
            _serviceCollection.AddTransient<WaveLengthUnitViewModel>();
            _serviceCollection.AddTransient<PowerUnitViewModel>();
            _serviceCollection.AddTransient<TemperatureViewModel>();
            _serviceCollection.AddTransient<TriggerSettingViewModel>();

            #endregion 硬件设定子项

            #endregion 硬件

            #region Measure

            _serviceCollection.AddTransient<WaveLengthMeasureViewModel>();

            #endregion Measure

            #region SystemSetting

            _serviceCollection.AddTransient<SystemSettingViewModel>();

            _serviceCollection.AddTransient<IPSettingViewModel>();
            _serviceCollection.AddTransient<GainViewModel>();
            _serviceCollection.AddTransient<AdcSettingViewModel>();

            #endregion SystemSetting

            #region Chart

            _serviceCollection.AddTransient<CavityChartViewModel>();
            _serviceCollection.AddTransient<WaveLengthChartViewModel>();

            #endregion Chart

            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public MainViewModel? MainViewModel => _serviceProvider?.GetService<MainViewModel>();

        #region Login

        public LoginViewModel? LoginViewModel => _serviceProvider?.GetService<LoginViewModel>();

        #endregion Login

        #region 下位机参数

        public LcParamViewModel? LcParamViewModel => _serviceProvider?.GetService<LcParamViewModel>();

        #endregion 下位机参数

        #region 硬件

        public HardwareSettingViewModel? HardwareParamViewModel => _serviceProvider?.GetService<HardwareSettingViewModel>();

        #region 硬件设定子项

        public AverageSettingViewModel? AverageSettingViewModel => _serviceProvider?.GetService<AverageSettingViewModel>();

        public CalibrationSettingViewModel? CalibrationSettingViewModel => _serviceProvider?.GetService<CalibrationSettingViewModel>();

        public ExposureSettingViewModel? ExposureSettingViewModel => _serviceProvider?.GetService<ExposureSettingViewModel>();

        public HardWareSnViewModel? HardWareSnViewModel => _serviceProvider?.GetService<HardWareSnViewModel>();

        public LaserModeViewModel? LaserModeViewModel => _serviceProvider?.GetService<LaserModeViewModel>();

        public LaserOperateViewModel? LaserOperateViewModel => _serviceProvider?.GetService<LaserOperateViewModel>();

        public WaveLengthUnitViewModel? WaveLengthUnitViewModel => _serviceProvider?.GetService<WaveLengthUnitViewModel>();

        public PowerUnitViewModel? PowerUnitViewModel => _serviceProvider?.GetService<PowerUnitViewModel>();

        public TriggerSettingViewModel? TriggerSettingViewModel => _serviceProvider?.GetService<TriggerSettingViewModel>();

        #endregion 硬件设定子项

        #region 状态

        public TemperatureViewModel? TemperatureViewModel => _serviceProvider?.GetService<TemperatureViewModel>();

        #endregion 状态

        #endregion 硬件

        #region Measure

        public WaveLengthMeasureViewModel? WaveLengthMeasureViewModel => _serviceProvider?.GetService<WaveLengthMeasureViewModel>();

        #endregion Measure

        #region SyetemSetting

        public IPSettingViewModel? IPSettingViewModel => _serviceProvider?.GetService<IPSettingViewModel>();

        public SystemSettingViewModel? SystemSettingViewModel => _serviceProvider?.GetService<SystemSettingViewModel>();

        public GainViewModel? GainViewModel => _serviceProvider?.GetService<GainViewModel>();

        public AdcSettingViewModel? AdcSettingViewModel => _serviceProvider?.GetService<AdcSettingViewModel>();

        #endregion SyetemSetting

        #region Chart

        public CavityChartViewModel? CavityChartViewModel => _serviceProvider?.GetService<CavityChartViewModel>();

        public WaveLengthChartViewModel? WaveLengthChartViewModel => _serviceProvider?.GetService<WaveLengthChartViewModel>();

        #endregion Chart
    }
}
using Microsoft.Extensions.DependencyInjection;
using System;
using Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel.SingleEntity;
using Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel.Integrated;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel
{
    public static class ViewModelLocator
    {
        private static IServiceProvider? _serviceProvider;

        static ViewModelLocator()
        {
            var _serviceCollection = new ServiceCollection();

            _serviceCollection.AddTransient<LcParamViewModel>();
            _serviceCollection.AddTransient<CompensationViewModel>();
            _serviceCollection.AddTransient<FizeauViewModel>();
            _serviceCollection.AddTransient<FreqStableViewModel>();
            _serviceCollection.AddTransient<MultiPeaksViewModel>();
            _serviceCollection.AddTransient<PowerParamViewModel>();

            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public static LcParamViewModel? LcParamViewModel => _serviceProvider?.GetService<LcParamViewModel>();

        public static CompensationViewModel? CompensationViewModel => _serviceProvider?.GetService<CompensationViewModel>();

        public static FizeauViewModel? FizeauViewModel => _serviceProvider?.GetService<FizeauViewModel>();

        public static FreqStableViewModel? FreqStableViewModel => _serviceProvider?.GetService<FreqStableViewModel>();

        public static MultiPeaksViewModel? MultiPeaksViewModel => _serviceProvider?.GetService<MultiPeaksViewModel>();

        public static PowerParamViewModel? PowerParamViewModel => _serviceProvider?.GetService<PowerParamViewModel>();
    }
}
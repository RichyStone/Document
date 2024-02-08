using System.Windows.Controls;
using Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.View.SingleEntity
{
    /// <summary>
    /// UcPowerParam.xaml 的交互逻辑
    /// </summary>
    public partial class UcPowerParam : UserControl
    {
        public UcPowerParam()
        {
            InitializeComponent();
            DataContext = ViewModelLocator.PowerParamViewModel;
        }
    }
}
using System.Windows.Controls;
using Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.View.Integrated
{
    /// <summary>
    /// UcLcParam.xaml 的交互逻辑
    /// </summary>
    public partial class UcLcParam : UserControl
    {
        public UcLcParam()
        {
            InitializeComponent();
            DataContext = ViewModelLocator.LcParamViewModel;
        }
    }
}
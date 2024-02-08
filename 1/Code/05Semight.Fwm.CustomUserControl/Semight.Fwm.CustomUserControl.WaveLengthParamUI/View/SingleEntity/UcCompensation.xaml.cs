using System.Windows.Controls;
using Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.View.SingleEntity
{
    /// <summary>
    /// UcCompensation.xaml 的交互逻辑
    /// </summary>
    public partial class UcCompensation : UserControl
    {
        public UcCompensation()
        {
            InitializeComponent();
            DataContext = ViewModelLocator.CompensationViewModel;
        }
    }
}
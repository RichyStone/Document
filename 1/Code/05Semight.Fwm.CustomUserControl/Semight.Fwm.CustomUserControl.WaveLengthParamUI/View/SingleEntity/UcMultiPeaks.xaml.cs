using System.Windows.Controls;
using Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.View.SingleEntity
{
    /// <summary>
    /// UcMultiPeaks.xaml 的交互逻辑
    /// </summary>
    public partial class UcMultiPeaks : UserControl
    {
        public UcMultiPeaks()
        {
            InitializeComponent();
            DataContext = ViewModelLocator.MultiPeaksViewModel;
        }
    }
}
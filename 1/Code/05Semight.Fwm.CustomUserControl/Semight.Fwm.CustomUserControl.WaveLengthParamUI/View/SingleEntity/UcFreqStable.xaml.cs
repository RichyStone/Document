using System.Windows.Controls;
using Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.View.SingleEntity
{
    /// <summary>
    /// UcFreqStable.xaml 的交互逻辑
    /// </summary>
    public partial class UcFreqStable : UserControl
    {
        public UcFreqStable()
        {
            InitializeComponent();
            DataContext = ViewModelLocator.FreqStableViewModel;
        }
    }
}
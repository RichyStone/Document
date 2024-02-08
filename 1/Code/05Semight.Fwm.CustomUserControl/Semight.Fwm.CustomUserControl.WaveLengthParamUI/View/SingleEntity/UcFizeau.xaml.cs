using System.Windows.Controls;
using Semight.Fwm.CustomUserControl.WaveLengthParam.ViewModel;

namespace Semight.Fwm.CustomUserControl.WaveLengthParam.View.SingleEntity
{
    /// <summary>
    /// UcFizeau.xaml 的交互逻辑
    /// </summary>
    public partial class UcFizeau : UserControl
    {
        public UcFizeau()
        {
            InitializeComponent();
            DataContext = ViewModelLocator.FizeauViewModel;
        }
    }
}
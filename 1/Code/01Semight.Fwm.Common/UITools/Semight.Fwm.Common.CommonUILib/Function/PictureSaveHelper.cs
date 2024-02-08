using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Semight.Fwm.Common.CommonUILib.Function
{
    public class PictureSaveHelper
    {
        public static void Save(FrameworkElement visual, string path)
        {
            System.IO.FileStream fs = new(path, System.IO.FileMode.Create);
            RenderTargetBitmap bmp = new((int)visual.ActualWidth + 10, (int)visual.ActualHeight + 10, 96d, 96d, PixelFormats.Pbgra32);
            bmp.Render(visual);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(fs);
            fs.Close();
        }
    }
}
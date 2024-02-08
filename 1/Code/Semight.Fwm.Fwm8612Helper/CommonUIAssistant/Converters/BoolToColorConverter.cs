using Semight.Fwm.Fwm8612Helper.Model.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Semight.Fwm.Fwm8612Helper.CommonUIAssistant.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = new SolidColorBrush();

            if (value is bool state)
            {
                if (state)
                    brush.Color = Color.FromArgb(255, 20, 100, 20);
                else
                    brush.Color = Color.FromArgb(255, 220, 30, 10);
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
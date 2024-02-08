using Semight.Fwm.Fwm8612Helper.Model.Enums;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Semight.Fwm.Fwm8612Helper.CommonUIAssistant.Converters
{
    public class StateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = new SolidColorBrush();

            if (value is HardWareState state)
            {
                switch (state)
                {
                    case HardWareState.Normal:
                        brush.Color = Color.FromArgb(255, 20, 100, 20);
                        break;

                    case HardWareState.Error:
                    case HardWareState.None:
                    default:
                        brush.Color = Color.FromArgb(255, 220, 30, 10);
                        break;
                }
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
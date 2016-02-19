using System;
using System.Globalization;
using System.Windows.Data;

namespace ED_CustomControls
{
    public class BrightnessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            return (int)(System.Convert.ToDouble(value) * 255);
            //return 255;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}

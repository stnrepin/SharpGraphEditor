using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Windows.Media;

namespace SharpGraphEditor.Converters
{
    public class ContrastColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var middleColor = Color.FromRgb(128, 128, 128);
            var color = ((SolidColorBrush)value).Color;
            if (color.R > middleColor.R &&
                color.G > middleColor.G &&
                color.B > middleColor.B)
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0)); // Black.
            }
            return new SolidColorBrush(Color.FromRgb(255, 255, 255)); // White.
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

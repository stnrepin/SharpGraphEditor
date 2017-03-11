using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SharpGraphEditor.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || parameter.ToString() == "0")
            {
                return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }
            else if (parameter.ToString() == "1")
            {
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new ArgumentException("invalid parameter of BooleanToVisibilityConverter");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || parameter.ToString() == "0")
            {
                return ((Visibility)value == Visibility.Collapsed) ? true : false;
            }
            else if (parameter.ToString() == "1")
            {
                return ((Visibility)value) == Visibility.Visible ? true : false;
            }
            throw new ArgumentException("invalid parameter of BooleanToVisibilityConverter");
        }
    }
}

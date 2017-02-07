using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SharpGraphEditor.Converters
{
    public class IsEquals : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return !values.Any(x => x != values[0]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

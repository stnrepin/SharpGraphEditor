using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using System.Windows.Data;

using SharpGraphEditor.Models;

namespace SharpGraphEditor.Converters
{
    public class CursorModeEnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = (CursorMode)value;
            return mode.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == false)
            {
                return CursorMode.Default;
            }
            if (Enum.TryParse(parameter.ToString(), out CursorMode res))
            {
                return res;
            }
            throw new ArgumentException("incorrect name of CursorMode");
        }
    }
}

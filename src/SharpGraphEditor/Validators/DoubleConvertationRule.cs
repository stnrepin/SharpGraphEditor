using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using System.Windows.Controls;

namespace SharpGraphEditor.Validators
{
    public class DoubleConvertationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var valueStr = value.ToString();
            if (valueStr.Length > 0)
            {
                if (Double.TryParse(valueStr, out double res))
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "Input must be rational number (max 15 symbols)");
        }
    }
}

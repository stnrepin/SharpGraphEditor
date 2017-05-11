using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SharpGraphEditor.Validators
{
    public class DoubleBetweenZeroAndOneRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Double.TryParse(value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
            {
                if (number >= 0 && number <= 1)
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "Input must be rational number between 0 and 1");
        }
    }
}

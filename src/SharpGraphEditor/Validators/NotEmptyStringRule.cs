using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using System.Windows.Controls;

namespace SharpGraphEditor.Validators
{
    public class NotEmptyStringRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (String.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Input string must be not empty");
            }
            return new ValidationResult(true, null);
        }
    }
}

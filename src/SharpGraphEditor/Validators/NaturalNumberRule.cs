using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SharpGraphEditor.Validators
{
    public class NaturalNumberRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Int32.TryParse(value?.ToString(), out int number))
            {
                if (number > 0)
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "Input number must be natural number");
        }
    }
}

using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Core.ViewModels.ValidationRules
{
    internal class RejectNullOrEmptyString : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string text && !string.IsNullOrWhiteSpace(text))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, Application.Current.FindResource("Lang-InputNothing").ToString());
            }
        }
    }
}

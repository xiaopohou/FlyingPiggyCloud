using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SixCloud.Core.ViewModels.ValidationRules
{
    internal class PhoneNumber : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string phoneNumber && !string.IsNullOrWhiteSpace(phoneNumber) && Regex.IsMatch(phoneNumber, "^[1]+[3,4,5,7,8,9]+\\d{9}$"))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, Application.Current.FindResource("Lang-InvalidPhoneNumber"));
            }
        }
    }
}

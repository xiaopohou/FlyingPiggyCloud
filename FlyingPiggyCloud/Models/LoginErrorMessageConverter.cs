using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FlyingPiggyCloud.Models
{
    public class LoginErrorMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isLoginError = (bool)value;
            if (isLoginError)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

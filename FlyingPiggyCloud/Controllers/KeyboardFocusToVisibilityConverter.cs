using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyingPiggyCloud.Controllers
{
    internal class KeyboardFocusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isKeyboardFocus = (bool)value;
            return isKeyboardFocus ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (System.Windows.Visibility)value;
            return visibility == System.Windows.Visibility.Visible;
        }
    }
}

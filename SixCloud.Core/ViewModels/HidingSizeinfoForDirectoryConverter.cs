using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SixCloud.Core.ViewModels
{
    public class HidingSizeinfoForDirectoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool directory)
            {
                return directory ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
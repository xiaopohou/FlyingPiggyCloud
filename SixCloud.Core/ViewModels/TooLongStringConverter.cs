using System;
using System.Globalization;
using System.Windows.Data;

namespace SixCloud.Core.ViewModels
{
    public class TooLongStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                var length = (parameter as int?) ?? 20;
                if (str.Length > length)
                {
                    return $"{str.Substring(0, 14)}...{str.Substring(str.Length - 3)}";
                }
                else
                {
                    return str;
                }
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
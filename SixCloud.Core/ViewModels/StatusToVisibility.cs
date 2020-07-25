using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SixCloud.Core.ViewModels
{
    public class StatusToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TransferTaskStatus)value) == TransferTaskStatus.Running ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyingPiggyCloud.Controllers
{
    internal class SizeCalculatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConverterToolKits.SizeCalculator((long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

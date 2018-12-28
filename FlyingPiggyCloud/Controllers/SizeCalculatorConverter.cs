using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyingPiggyCloud.Controllers
{
    /// <summary>
    /// 将long类型的字节数转化为适合单位的字符串形式，不支持反向转换！
    /// </summary>
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

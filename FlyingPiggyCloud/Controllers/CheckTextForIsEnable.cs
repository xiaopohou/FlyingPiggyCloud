using System;
using System.Globalization;
using System.Windows.Data;

namespace FlyingPiggyCloud.Controllers
{
    /// <summary>
    /// 通过字符串反馈布尔值，可用于校验输入合法性
    /// </summary>
    internal class CheckTextForIsEnable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string Value = (string)value;
            if ((string)value == "")
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool Value = (bool)value;
            if (Value)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
    }
}

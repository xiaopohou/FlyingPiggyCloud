using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FlyingPiggyCloud.Controllers
{
    public static class ConverterToolKits
    {
        /// <summary>
        /// 根据指定字符串依据UTF-8编码计算其md5小写值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UserMd5(string input)
        {
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            var x = sBuilder.ToString();
            return x;
        }

        /// <summary>
        /// 通过字符串反馈布尔值，可用于校验输入合法性
        /// </summary>
        internal class CheckTextForIsEnable : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string Value = (string)value;
                if ((string)value == "")
                    return false;
                return true;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool Value = (bool)value;
                if (Value)
                    return "1";
                else
                    return "0";
            }
        }

    }
}

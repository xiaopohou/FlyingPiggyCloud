using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Data;

namespace FlyingPiggyCloud.Controllers
{
    public static class ConverterToolKits
    {
        private const float V = 1024f;

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
            string x = sBuilder.ToString();
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

        /// <summary>
        /// 将单位为字节的文件尺寸表示为可读性良好的字符串
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        internal static string SizeCalculator(long Size)
        {
            if (Size / V < 1)
            {
                return ((float)Size).ToString("F2") + "B";
            }
            else if (Size / V / V < 1)
            {
                return (Size / V).ToString("F2") + "KB";
            }
            else if (Size / V / V / V < 1)
            {
                return (Size / V / V).ToString("F2") + "MB";
            }
            else
            {
                return (Size / V / V / V).ToString("F2") + "GB";
            }
        }


    }
}

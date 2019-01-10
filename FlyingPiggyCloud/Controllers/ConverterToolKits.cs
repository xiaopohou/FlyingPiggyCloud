using System.Security.Cryptography;
using System.Text;

namespace FlyingPiggyCloud.Controllers
{
    internal static class Calculators
    {
        private const float V = 1024f;

        /// <summary>
        /// 根据指定字符串依据UTF-8编码计算其md5小写值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string UserMd5(string input)
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
        /// 将单位为字节的文件尺寸表示为可读性良好的字符串
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        internal static string SizeCalculator(long size)
        {
            if (size / V < 1)
            {
                return ((float)size).ToString("F2") + "B";
            }
            else if (size / V / V < 1)
            {
                return (size / V).ToString("F2") + "KB";
            }
            else if (size / V / V / V < 1)
            {
                return (size / V / V).ToString("F2") + "MB";
            }
            else
            {
                return (size / V / V / V).ToString("F2") + "GB";
            }
        }
    }
}

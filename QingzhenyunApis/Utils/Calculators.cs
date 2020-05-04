using System;
using System.Security.Cryptography;
using System.Text;

namespace QingzhenyunApis.Utils
{
    public static class Calculators
    {
        private const float V = 1024f;
        /// <summary>
        /// 将单位为字节的文件尺寸表示为可读性良好的字符串
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string SizeCalculator(long size)
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

        public static string UnixTimeStampConverter(long UnixTimeStamp)
        {
            return TimeZoneInfo.ConvertTimeFromUtc((new DateTime(1970, 1, 1)).AddMilliseconds(UnixTimeStamp), TimeZoneInfo.Local).ToString("yyyy/MM/dd HH:mm:ss");
            //return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(UnixTimeStamp).ToString("yyyy/MM/dd HH:mm:ss");
        }

        /// <summary>
        /// 用户输入的密码通过此方法转换为MD5值
        /// </summary>
        /// <param name="input">用户输入</param>
        /// <returns></returns>
        public static string UserMd5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sBuilder.Append(bytes[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        internal class Base64
        {
            /// <summary>
            /// 字符串 URL 安全 Base64 编码
            /// </summary>
            /// <param name="text">源字符串</param>
            /// <returns>编码</returns>
            public static string UrlSafeBase64Encode(string text)
            {
                return UrlSafeBase64Encode(Encoding.UTF8.GetBytes(text));
            }

            /// <summary>
            /// URL 安全的 Base64 编码
            /// </summary>
            /// <param name="data">需要编码的字节数据</param>
            /// <returns>编码</returns>
            public static string UrlSafeBase64Encode(byte[] data)
            {
                return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_');
            }

            /// <summary>
            /// bucket:key 编码
            /// == Python SDK: def entry(bucket, key)
            /// </summary>
            /// <param name="bucket">空间名称</param>
            /// <param name="key">文件 Key</param>
            /// <returns>编码</returns>
            public static string UrlSafeBase64Encode(string bucket, string key)
            {
                return UrlSafeBase64Encode(bucket + ":" + key);
            }

            /// <summary>
            /// Base64解码
            /// </summary>
            /// <param name="text">待解码的字符串</param>
            /// <returns>已解码字节</returns>
            public static byte[] UrlSafeBase64DecodeByte(string text)
            {
                return Convert.FromBase64String(text.Replace('-', '+').Replace('_', '/'));
            }

            /// <summary>
            /// Base64解码
            /// </summary>
            /// <param name="text">待解码的字符串</param>
            /// <returns>已解码字符串</returns>
            public static string UrlSafeBase64Decode(string text)
            {
                return Encoding.UTF8.GetString(UrlSafeBase64DecodeByte(text));
            }

            public static string Base64Encode(string plainText)
            {
                byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }

            public static string Base64Decode(string base64EncodedData)
            {
                byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
        }
    }
}

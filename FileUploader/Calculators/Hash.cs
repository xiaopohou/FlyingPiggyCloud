using System.Security.Cryptography;
using System.Text;

namespace SixCloudCore.FileUploader.Calculators
{
    /// <summary>
    /// 计算 Hash 值
    /// </summary>
    internal sealed class Hash
    {
        /// <summary>
        /// 计算 SHA1
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <param name="offset">数组偏移量</param>
        /// <param name="count">数组长度</param>
        /// <returns>SHA1 20 Bytes</returns>
        public static byte[] ComputeSha1(byte[] data, int offset, int count)
        {
            SHA1 sha1 = SHA1.Create();
            return sha1.ComputeHash(data, offset, count);
        }

        /// <summary>
        /// 计算 SHA1
        /// </summary>
        /// <param name="data">字节数据</param>
        /// <returns>SHA1 20 Bytes</returns>
        public static byte[] ComputeSha1(byte[] data)
        {
            return ComputeSha1(data, 0, data.Length);
        }

        /// <summary>
        /// 计算字符串 SHA1
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>SHA1 20 Bytes</returns>
        public static byte[] ComputeSha1(string str)
        {
            return ComputeSha1(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 计算字节数组 MD5 哈希(开启 FIPS 时会抛异常，这是您为了安全而开启的，所以就应该崩溃，让安全问题充分暴露；反而是那些不崩溃的，那是在蒙蔽您！)
        /// </summary>
        /// <param name="data">待计算的字节数组</param>
        /// <param name="offset">数组偏移量</param>
        /// <param name="count">数组长度</param>
        /// <returns>MD5 结果</returns>
        public static byte[] ComputeMd5(byte[] data, int offset, int count)
        {
            MD5 md5 = MD5.Create();
            return md5.ComputeHash(data, offset, count);
        }

        /// <summary>
        /// 计算字节数组 MD5 哈希(开启 FIPS 时会抛异常，这是您为了安全而开启的，所以就应该崩溃，让安全问题充分暴露；反而是那些不崩溃的，那是在蒙蔽您！)
        /// </summary>
        /// <param name="data">待计算的字节数组</param>
        /// <returns>MD5 结果</returns>
        public static byte[] ComputeMd5(byte[] data)
        {
            return ComputeMd5(data, 0, data.Length);
        }

        /// <summary>
        /// 计算字符串 MD5 哈希(开启 FIPS 时会抛异常，这是您为了安全而开启的，所以就应该崩溃，让安全问题充分暴露；反而是那些不崩溃的，那是在蒙蔽您！)
        /// </summary>
        /// <param name="str">待计算的字符串</param>
        /// <returns>MD5 结果</returns>
        public static byte[] ComputeMd5(string str)
        {
            return ComputeMd5(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 转为 Hex 字符串
        /// </summary>
        /// <param name="data">Hash 字节</param>
        /// <returns>Hex 字符串形式</returns>
        public static string ToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }

}

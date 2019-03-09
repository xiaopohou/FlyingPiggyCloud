using System;

namespace SixCloud.Controllers
{
    internal static class Calculators
    {
        private const float V = 1024f;
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

        internal static string UnixTimeStampConverter(long UnixTimeStamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds(UnixTimeStamp).ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}

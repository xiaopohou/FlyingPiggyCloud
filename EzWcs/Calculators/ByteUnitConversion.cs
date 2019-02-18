namespace EzWcs.Calculators
{
    internal class ByteUnitConversion
    {
        private const float V = 1024f;
        public static string AutomaticInit(long bytes)
        {
            if (bytes / V < 1)
            {
                return ((float)bytes).ToString("F2") + "B";
            }
            else if (bytes / V / V < 1)
            {
                return (bytes / V).ToString("F2") + "KB";
            }
            else if (bytes / V / V / V < 1)
            {
                return (bytes / V / V).ToString("F2") + "MB";
            }
            else
            {
                return (bytes / V / V / V).ToString("F2") + "GB";
            }
        }
    }
}

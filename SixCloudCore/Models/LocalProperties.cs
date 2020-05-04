using Microsoft.Win32;
using SixCloudCore.Views;

namespace SixCloudCore.Models
{
    internal class LocalProperties
    {
        private const string DomainSubKey = @"Software\EzSuit\SixCloud";

        public static RegistryKey ApplicationDictionary { get; } = Registry.CurrentUser.CreateSubKey(DomainSubKey);

        public static string Token
        {
            get => (string)ApplicationDictionary.GetValue("Token");
            set => ApplicationDictionary.SetValue("Token", value);
        }

        public static bool IsSavedPassword
        {
            get => ApplicationDictionary.GetValue("IsSavedPassword") == null ? false : bool.Parse((string)ApplicationDictionary.GetValue("IsSavedPassword"));
            set => ApplicationDictionary.SetValue("IsSavedPassword", value);
        }

        public static bool IsAutoLogin
        {
            get => ApplicationDictionary.GetValue("IsAutoLogin") == null ? false : bool.Parse((string)ApplicationDictionary.GetValue("IsAutoLogin"));
            set => ApplicationDictionary.SetValue("IsAutoLogin", value);
        }

        public static string CountryCode
        {
            get => (string)ApplicationDictionary.GetValue("CountryCode") ?? "(86)中国大陆";
            set => ApplicationDictionary.SetValue("CountryCode", value);
        }

    }
}

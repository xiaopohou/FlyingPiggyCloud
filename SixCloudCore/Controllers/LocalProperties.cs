using Microsoft.Win32;

namespace SixCloudCore.Controllers
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

    }
}

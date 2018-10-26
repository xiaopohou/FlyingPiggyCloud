using Microsoft.Win32;

namespace FlyingPiggyCloud.Controllers
{
    internal static class RegistryManager
    {
        private const string DomainSubKey = @"Software\EzSuit\FlyingPiggyCloud";

        public static RegistryKey ApplicationDictionary { get; } = Registry.CurrentUser.CreateSubKey(DomainSubKey);

        public static string UserName
        {
            get => (string)ApplicationDictionary.GetValue("UserName");
            set => ApplicationDictionary.SetValue("UserName", value);
        }

        public static string Password
        {
            get => (string)ApplicationDictionary.GetValue("Password");
            set => ApplicationDictionary.SetValue("Password", value);
        }

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

        public static Views.PageNavigate DefaultPage
        {
            get => ApplicationDictionary.GetValue("DefaultPage") == null ? Views.PageNavigate.Root : (Views.PageNavigate)System.Enum.Parse(typeof(Views.PageNavigate),(string)ApplicationDictionary.GetValue("DefaultPage"));
            set => ApplicationDictionary.SetValue("DefaultPage", value);
        }

        //public static string Token
        //{
        //    get => (string)ApplicationDictionary.GetValue("Token");
        //    set => ApplicationDictionary.SetValue("Token", value);
        //}

        //public static string Token
        //{
        //    get => (string)ApplicationDictionary.GetValue("Token");
        //    set => ApplicationDictionary.SetValue("Token", value);
        //}

        static RegistryManager()
        {
        }
    }
}

using Microsoft.Win32;

namespace SixCloudCore.Controllers
{
    internal class LocalProperties
    {
        private const string DomainSubKey = @"Software\EzSuit\SixCloud";

        public static RegistryKey ApplicationDictionary { get; } = Registry.CurrentUser.CreateSubKey(DomainSubKey);

        public static string Token
        {
            get => (string)ApplicationDictionary.GetValue(nameof(Token));
            set => ApplicationDictionary.SetValue(nameof(Token), value);
        }

        public FileViewType FileViewType
        {
            get => (FileViewType)(ApplicationDictionary.GetValue(nameof(FileViewType)) ?? FileViewType.DetailsList);
            set => ApplicationDictionary.SetValue(nameof(FileViewType), value);
        }
    }

    internal enum FileViewType
    {
        DetailsList = 0,
        LargeIconList = 1,
        SmallIconList = 2,
        TitleList = 3
    }
}

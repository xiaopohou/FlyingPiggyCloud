using Microsoft.Win32;
using System.Windows.Media;

namespace SixCloud.Core.Controllers
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

        public static FileViewType FileViewType
        {
            get => (FileViewType)(ApplicationDictionary.GetValue(nameof(FileViewType)) ?? FileViewType.DetailsList);
            set => ApplicationDictionary.SetValue(nameof(FileViewType), value);
        }
        public static Color? AccentColor
        {
            get => (Color?)ApplicationDictionary.GetValue(nameof(AccentColor));
            set => ApplicationDictionary.SetValue(nameof(AccentColor), value);
        }
        public static Color? ForegroundColor
        {
            get => (Color?)ApplicationDictionary.GetValue(nameof(ForegroundColor));
            set => ApplicationDictionary.SetValue(nameof(ForegroundColor), value);
        }
        public static Color? BackgroundColor
        {
            get => (Color?)ApplicationDictionary.GetValue(nameof(BackgroundColor));
            set => ApplicationDictionary.SetValue(nameof(BackgroundColor), value);
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

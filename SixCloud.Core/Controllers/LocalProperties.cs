using Microsoft.Win32;
using QingzhenyunApis.Exceptions;
using System;
using System.Windows.Media;

namespace SixCloud.Core.Controllers
{
    internal class LocalProperties
    {
        private const string DomainSubKey = @"Software\EzSuit\SixCloud";

        public static RegistryKey ApplicationDictionary { get; } = Registry.CurrentUser.CreateSubKey(DomainSubKey);

        public static string Token
        {
            get
            {
                try
                {
                    return (string)ApplicationDictionary.GetValue(nameof(Token));
                }
                catch (Exception ex)
                {
                    ex.ToSentry().TreatedBy(nameof(LocalProperties)).Submit();
                    Token = string.Empty;
                    return "";
                }
            }

            set => ApplicationDictionary.SetValue(nameof(Token), value);
        }
        public static string Lang
        {
            get
            {
                try
                {
                    return (string)ApplicationDictionary.GetValue(nameof(Lang));
                }
                catch (Exception ex)
                {
                    ex.ToSentry().TreatedBy(nameof(LocalProperties)).Submit();
                    Lang = "zh-CN";
                    return Lang;
                }
            }

            set => ApplicationDictionary.SetValue(nameof(Lang), value);
        }

        public static FileViewType FileViewType
        {
            get => (FileViewType)(ApplicationDictionary.GetValue(nameof(FileViewType)) ?? FileViewType.DetailsList);
            set => ApplicationDictionary.SetValue(nameof(FileViewType), value);
        }
        public static Color? AccentColor
        {
            get => ApplicationDictionary.GetValue(nameof(AccentColor)) is string colorString ? (string.IsNullOrEmpty(colorString) ? null : ColorConverter.ConvertFromString(colorString) as Color?) : null;
            set => ApplicationDictionary.SetValue(nameof(AccentColor), value == null ? "" : (object)value);
        }
        public static Color? ForegroundColor
        {
            get => ApplicationDictionary.GetValue(nameof(ForegroundColor)) is string colorString ? (string.IsNullOrEmpty(colorString) ? null : ColorConverter.ConvertFromString(colorString) as Color?) : null;
            set => ApplicationDictionary.SetValue(nameof(ForegroundColor), value == null ? "" : (object)value);
        }
        public static Color? BackgroundColor
        {
            get => ApplicationDictionary.GetValue(nameof(BackgroundColor)) is string colorString ? (string.IsNullOrEmpty(colorString) ? null : ColorConverter.ConvertFromString(colorString) as Color?) : null;
            set => ApplicationDictionary.SetValue(nameof(BackgroundColor), value == null ? "" : (object)value);
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

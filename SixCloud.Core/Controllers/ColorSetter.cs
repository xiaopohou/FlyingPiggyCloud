using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SixCloud.Core.Controllers
{
    internal class ColorSetter
    {
        public static Color AccentColor { get => (Application.Current.Resources["ImmersiveSystemAccentBrush"] as SolidColorBrush).Color; set => SetAccentColor(value); }

        private static void SetAccentColor(Color color)
        {
            Application.Current.Resources["ImmersiveSystemAccentBrushLight3"] = new SolidColorBrush(Multiply(color, 0.3));
            Application.Current.Resources["ImmersiveSystemAccentBrushLight2"] = new SolidColorBrush(Multiply(color, 0.2));
            Application.Current.Resources["ImmersiveSystemAccentBrushLight1"] = new SolidColorBrush(Multiply(color, 0.1));
            Application.Current.Resources["ImmersiveSystemAccentBrush"] = new SolidColorBrush(color);
            Application.Current.Resources["ImmersiveSystemAccentBrushDark1"] = new SolidColorBrush(Multiply(color, -0.1));
            Application.Current.Resources["ImmersiveSystemAccentBrushDark2"] = new SolidColorBrush(Multiply(color, -0.2));
            Application.Current.Resources["ImmersiveSystemAccentBrushDark3"] = new SolidColorBrush(Multiply(color, -0.3));

            Application.Current.Resources["PrimaryHueLightBrush"] = new SolidColorBrush(Multiply(color, 0.1));
            Application.Current.Resources["PrimaryHueMidBrush"] = new SolidColorBrush(color);
            Application.Current.Resources["PrimaryHueDarkBrush"] = new SolidColorBrush(Multiply(color, -0.1));

            LocalProperties.AccentColor = color;

        }

        public static Color ForegroundColor { get => (Application.Current.Resources["MainForegroundBrush"] as SolidColorBrush).Color; set => SetForegroundColor(value); }
        private static void SetForegroundColor(Color color)
        {
            Application.Current.Resources["MainForegroundBrush"] = new SolidColorBrush(color);
            LocalProperties.ForegroundColor = color;
        }

        public static Color BackgroundColor { get => (Application.Current.Resources["MainBackgroundBrush"] as SolidColorBrush).Color; set => SetBackgroundColor(value); }
        private static void SetBackgroundColor(Color color)
        {
            Application.Current.Resources["MainBackgroundBrush"] = new SolidColorBrush(color);
            LocalProperties.BackgroundColor = color;
        }

        private static Color Multiply(Color color, double coefficient)
        {
            if (coefficient > 0)
            {
                return Color.FromArgb((byte)(color.A + (255 - color.A) * coefficient), (byte)(color.R + (255 - color.R) * coefficient), (byte)(color.G + (255 - color.G) * coefficient), (byte)(color.B + (255 - color.B) * coefficient));
            }
            else if (coefficient < 0)
            {
                return Color.FromArgb((byte)(color.A + color.A * coefficient), (byte)(color.R + color.R * coefficient), (byte)(color.G + color.G * coefficient), (byte)(color.B + color.B * coefficient));
            }
            else
            {
                return color;
            }
        }
    }

    internal class LanguageSetter
    {
        public static void SetLanguage(string lang = "zh-CN")
        {
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();
            var requestedCulture = $"pack://application:,,,/SixCloud.Core.LocalizationResources;component/{lang}.xaml";

            var defaultCulture = $"pack://application:,,,/SixCloud.Core.LocalizationResources;component/en-US.xaml";
            var defaultPackage = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(defaultCulture));

            if (defaultPackage != default)
            {
                Application.Current.Resources.MergedDictionaries.Remove(defaultPackage);
                Application.Current.Resources.MergedDictionaries.Add(defaultPackage);
            }

            if (requestedCulture != defaultCulture)
            {
                var requestPackage = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));

                if (requestPackage != default)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(requestPackage);
                    Application.Current.Resources.MergedDictionaries.Add(requestPackage);
                }
            }

            LocalProperties.Lang = lang;
        }

        public static IEnumerable<string> AvailableLanguage()
        {
            return from package in Application.Current.Resources.MergedDictionaries.ToList()
                   where package.Source.OriginalString.StartsWith("pack://application:,,,/SixCloud.Core.LocalizationResources;component/")
                   let originalString = package.Source.OriginalString
                   orderby originalString ascending
                   select originalString.Substring(originalString.Length - 10, 5);
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> SwitchLanguage()
        {
            while (true)
            {
                foreach (var lang in AvailableLanguage())
                {
                    SetLanguage(lang);
                    yield return lang;
                }
            }
        }
    }
}

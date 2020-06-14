using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SixCloud.Core.Controllers
{
    class ColorSetter
    {
        public static void SetAccentColor(Color color)
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

        public static void SetForegroundColor(Color color)
        {
            Application.Current.Resources["MainForegroundBrush"] = new SolidColorBrush(color);
            LocalProperties.ForegroundColor = color;
        }
        public static void SetBackgroundColor(Color color)
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
}

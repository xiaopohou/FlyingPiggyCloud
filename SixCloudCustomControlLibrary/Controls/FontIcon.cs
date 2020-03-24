using System.Windows;
using System.Windows.Controls;

namespace CustomControls.Controls
{
    /// <summary>
    /// 字体图标控件
    /// </summary>
    public class FontIcon : Control
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(FontIcon), new PropertyMetadata('\uf111'.ToString()));
        public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }

        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(FontIcon), new PropertyMetadata(16d));
        public double IconSize { get => (double)GetValue(IconSizeProperty); set => SetValue(IconSizeProperty, value); }

        static FontIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(typeof(FontIcon)));
        }
    }
}

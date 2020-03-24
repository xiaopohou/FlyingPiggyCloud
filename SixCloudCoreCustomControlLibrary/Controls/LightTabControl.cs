using System.Windows;
using System.Windows.Controls;

namespace CustomControls.Controls
{
    public class LightTabControl : TabControl
    {
        static LightTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LightTabControl), new FrameworkPropertyMetadata(typeof(LightTabControl)));
        }
    }

    public class LightTabItem : TabItem
    {
        public static readonly DependencyProperty FontIconProperty = DependencyProperty.Register("FontIcon", typeof(string), typeof(LightTabItem), new PropertyMetadata('\uf111'.ToString()));

        public string FontIcon { get => (string)GetValue(FontIconProperty); set => SetValue(FontIconProperty, value); }

        static LightTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LightTabItem), new FrameworkPropertyMetadata(typeof(LightTabItem)));
        }
    }
}
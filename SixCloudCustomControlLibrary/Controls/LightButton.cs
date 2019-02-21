using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SixCloudCustomControlLibrary.Controls
{
    public class LightButton : ButtonBase
    {
        public static DependencyProperty FontIconProperty = DependencyProperty.Register("FontIcon", typeof(string), typeof(LightButton), new PropertyMetadata('\uf111'.ToString()));
        public string FontIcon { get => (string)GetValue(FontIconProperty); set => SetValue(FontIconProperty, value); }
        
        static LightButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LightButton), new FrameworkPropertyMetadata(typeof(LightButton)));
        }
    }

}

using System.Windows;
using System.Windows.Controls;

namespace CustomControls.Controls
{
    public class MetroScrollViewer : ScrollViewer
    {
        public static readonly DependencyProperty FloatProperty = DependencyProperty.Register("Float", typeof(bool), typeof(MetroScrollViewer));
        public static readonly DependencyProperty AutoLimitMouseProperty = DependencyProperty.Register("AutoLimitMouse", typeof(bool), typeof(MetroScrollViewer));
        public static readonly DependencyProperty VerticalMarginProperty = DependencyProperty.Register("VerticalMargin", typeof(Thickness), typeof(MetroScrollViewer));
        public static readonly DependencyProperty HorizontalMarginProperty = DependencyProperty.Register("HorizontalMargin", typeof(Thickness), typeof(MetroScrollViewer));

        public bool Float { get => (bool)GetValue(FloatProperty); set => SetValue(FloatProperty, value); }
        public bool AutoLimitMouse { get => (bool)GetValue(AutoLimitMouseProperty); set => SetValue(AutoLimitMouseProperty, value); }
        public Thickness VerticalMargin { get => (Thickness)GetValue(VerticalMarginProperty); set => SetValue(VerticalMarginProperty, value); }
        public Thickness HorizontalMargin { get => (Thickness)GetValue(HorizontalMarginProperty); set => SetValue(HorizontalMarginProperty, value); }

        public MetroScrollViewer()
        {

        }

        static MetroScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroScrollViewer), new FrameworkPropertyMetadata(typeof(MetroScrollViewer)));
        }

    }
}
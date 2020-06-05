using System.Windows;
using System.Windows.Controls;

namespace CustomControls.Controls
{
    public class FluentTabItem : TabItem
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), typeof(FluentTabItem));
        public static readonly DependencyProperty IconMoveProperty = DependencyProperty.Register(nameof(IconMove), typeof(string), typeof(FluentTabItem));

        public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
        public string IconMove { get => (string)GetValue(IconMoveProperty); set => SetValue(IconMoveProperty, value); }

        static FluentTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FluentTabItem), new FrameworkPropertyMetadata(typeof(FluentTabItem)));
        }
    }
}

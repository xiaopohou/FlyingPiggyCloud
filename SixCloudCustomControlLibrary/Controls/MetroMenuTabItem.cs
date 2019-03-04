using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SixCloudCustomControlLibrary.Controls
{
    public class MetroMenuTabItem : TabItem
    {
        public static readonly DependencyProperty IconProperty = ElementBase.Property<MetroMenuTabItem, string>(nameof(IconProperty), null);
        public static readonly DependencyProperty IconMoveProperty = ElementBase.Property<MetroMenuTabItem, string>(nameof(IconMoveProperty), null);
        public static readonly DependencyProperty TextHorizontalAlignmentProperty = ElementBase.Property<MetroMenuTabItem, HorizontalAlignment>(nameof(TextHorizontalAlignmentProperty), HorizontalAlignment.Right);

        public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
        public string IconMove { get => (string)GetValue(IconMoveProperty); set => SetValue(IconMoveProperty, value); }
        public HorizontalAlignment TextHorizontalAlignment { get => (HorizontalAlignment)GetValue(TextHorizontalAlignmentProperty); set => SetValue(TextHorizontalAlignmentProperty, value); }

        static MetroMenuTabItem()
        {
            ElementBase.DefaultStyle<MetroMenuTabItem>(DefaultStyleKeyProperty);
        }
    }
}
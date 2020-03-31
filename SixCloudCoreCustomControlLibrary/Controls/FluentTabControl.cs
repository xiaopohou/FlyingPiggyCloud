using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomControls.Controls
{
    /// <summary>
    /// Fluent风格的标签控件
    /// </summary>
    public class FluentTabControl : TabControl
    {
        public static DependencyProperty TabItemsContainerWidthProperty = DependencyProperty.Register(nameof(TabItemsContainerWidth), typeof(double), typeof(FluentTabControl));

        public double TabItemsContainerWidth { get => (double)GetValue(TabItemsContainerWidthProperty); set => SetValue(TabItemsContainerWidthProperty, value); }

        
        static FluentTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FluentTabControl), new FrameworkPropertyMetadata(typeof(FluentTabControl)));
        }
    }
}

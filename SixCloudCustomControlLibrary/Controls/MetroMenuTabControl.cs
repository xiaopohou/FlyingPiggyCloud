using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SixCloudCustomControlLibrary.Controls
{
    public class MetroMenuTabControl : TabControl
    {
        public static readonly DependencyProperty TabPanelVerticalAlignmentProperty = DependencyProperty.Register("TabPanelVerticalAlignment", typeof(VerticalAlignment), typeof(MetroMenuTabControl), new PropertyMetadata(VerticalAlignment.Top));
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(Thickness), typeof(MetroMenuTabControl), new PropertyMetadata(new Thickness(0)));
        public static readonly DependencyProperty IconModeProperty = DependencyProperty.Register("IconMode", typeof(bool), typeof(MetroMenuTabControl), new PropertyMetadata(false, OnIconModeChanged));
        public static readonly DependencyProperty IconModeButtonVisibilityProperty = DependencyProperty.Register("IconModeButtonVisibility", typeof(Visibility), typeof(MetroMenuTabControl), new PropertyMetadata(Visibility.Visible));
        public static void OnIconModeChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is MetroMenuTabControl metroMenuTabControl)
            {
                metroMenuTabControl.GoToState();
            }
        }

        public static RoutedUICommand IconModeClickCommand = new RoutedUICommand(nameof(IconModeClickCommand), nameof(IconModeClickCommand), typeof(MetroMenuTabControl));

        public VerticalAlignment TabPanelVerticalAlignment { get => (VerticalAlignment)GetValue(TabPanelVerticalAlignmentProperty); set => SetValue(TabPanelVerticalAlignmentProperty, value); }
        public Thickness Offset { get => (Thickness)GetValue(OffsetProperty); set => SetValue(OffsetProperty, value); }
        public bool IconMode { get => (bool)GetValue(IconModeProperty); set => SetValue(IconModeProperty, value); }
        public Visibility IconModeButtonVisibility { get => (Visibility)GetValue(IconModeButtonVisibilityProperty); set => SetValue(IconModeButtonVisibilityProperty, value); }

        private void GoToState()
        {
            VisualStateManager.GoToState(this, IconMode ? "EnterIconMode" : "ExitIconMode", false);
        }

        private void SelectionState()
        {
            if (IconMode)
            {
                VisualStateManager.GoToState(this, "SelectionStartIconMode", false);
                VisualStateManager.GoToState(this, "SelectionEndIconMode", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "SelectionStart", false);
                VisualStateManager.GoToState(this, "SelectionEnd", false);
            }
        }

        public MetroMenuTabControl()
        {
            Loaded += delegate { GoToState(); VisualStateManager.GoToState(this, IconMode ? "SelectionLoadedIconMode" : "SelectionLoaded", false); };
            SelectionChanged += delegate (object sender, SelectionChangedEventArgs e) { if (e.Source is MetroMenuTabControl) { SelectionState(); } };
            CommandBindings.Add(new CommandBinding(IconModeClickCommand, delegate { IconMode = !IconMode; GoToState(); }));

            //Utility.Refresh(this);
        }

        static MetroMenuTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroMenuTabControl), new FrameworkPropertyMetadata(typeof(MetroMenuTabControl)));
        }
    }

    public class MetroFocusButton : Button
    {
        public static readonly DependencyProperty MouseMoveForegroundProperty = ElementBase.Property<MetroFocusButton, Brush>(nameof(MouseMoveForegroundProperty));
        public static readonly DependencyProperty MouseMoveBorderBrushProperty = ElementBase.Property<MetroFocusButton, Brush>(nameof(MouseMoveBorderBrushProperty));
        public static new readonly DependencyProperty BorderThicknessProperty = ElementBase.Property<MetroFocusButton, double>(nameof(BorderThicknessProperty));
        public static readonly DependencyProperty MouseMoveBorderThicknessProperty = ElementBase.Property<MetroFocusButton, double>(nameof(MouseMoveBorderThicknessProperty));
        public static readonly DependencyProperty StrokeDashArrayProperty = ElementBase.Property<MetroFocusButton, DoubleCollection>(nameof(StrokeDashArrayProperty));
        public static readonly DependencyProperty MouseMoveStrokeDashArrayProperty = ElementBase.Property<MetroFocusButton, DoubleCollection>(nameof(MouseMoveStrokeDashArrayProperty));
        public static readonly DependencyProperty CornerRadiusProperty = ElementBase.Property<MetroFocusButton, double>(nameof(CornerRadiusProperty));

        public Brush MouseMoveForeground { get => (Brush)GetValue(MouseMoveForegroundProperty); set => SetValue(MouseMoveForegroundProperty, value); }
        public Brush MouseMoveBorderBrush { get => (Brush)GetValue(MouseMoveBorderBrushProperty); set => SetValue(MouseMoveBorderBrushProperty, value); }
        public new double BorderThickness { get => (double)GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }
        public double MouseMoveBorderThickness { get => (double)GetValue(MouseMoveBorderThicknessProperty); set => SetValue(MouseMoveBorderThicknessProperty, value); }
        public DoubleCollection StrokeDashArray { get => (DoubleCollection)GetValue(StrokeDashArrayProperty); set => SetValue(StrokeDashArrayProperty, value); }
        public DoubleCollection MouseMoveStrokeDashArray { get => (DoubleCollection)GetValue(MouseMoveStrokeDashArrayProperty); set => SetValue(MouseMoveStrokeDashArrayProperty, value); }
        public double CornerRadius { get => (double)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

        static MetroFocusButton()
        {
            ElementBase.DefaultStyle<MetroFocusButton>(DefaultStyleKeyProperty);
        }
    }
}
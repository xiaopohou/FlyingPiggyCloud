using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SixCloudCustomControlLibrary.Controls
{
    public class MetroWindow : Window
    {
        public static readonly DependencyProperty IsSubWindowShowProperty = DependencyProperty.Register("IsSubWindowShow", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static new readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty MetroWindowLayoutProperty = DependencyProperty.Register("MetroWindowLayout", typeof(MetroWindowLayout), typeof(MetroWindow), new PropertyMetadata(MetroWindowLayout.Normal));

        public bool IsSubWindowShow { get => (bool)GetValue(IsSubWindowShowProperty); set => SetValue(IsSubWindowShowProperty, value); }
        public new Brush BorderBrush { get => (Brush)GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
        public Brush TitleForeground { get => (Brush)GetValue(TitleForegroundProperty); set => SetValue(TitleForegroundProperty, value); }
        public MetroWindowLayout MetroWindowLayout { get => (MetroWindowLayout)GetValue(MetroWindowLayoutProperty); set => SetValue(MetroWindowLayoutProperty, value); }

        public object ReturnValue { get; set; } = null;
        public bool EscClose { get; set; } = false;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AllowsTransparency = false;
            if (WindowStyle == WindowStyle.None)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        public MetroWindow()
        {
            SizeToContent sizeToContent = SizeToContent.Manual;
            Loaded += (ss, ee) =>
            {
                sizeToContent = SizeToContent;
            };
            ContentRendered += (ss, ee) =>
            {
                SizeToContent = SizeToContent.Manual;
                Width = ActualWidth;
                Height = ActualHeight;
                SizeToContent = sizeToContent;
            };

            KeyUp += delegate (object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Escape && EscClose)
                {
                    Close();
                }
            };
            StateChanged += delegate
              {
                  if (ResizeMode == ResizeMode.CanMinimize || ResizeMode == ResizeMode.NoResize)
                  {
                      if (WindowState == WindowState.Maximized)
                      {
                          WindowState = WindowState.Normal;
                      }
                  }
              };
        }

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }
    }

    public enum MetroWindowLayout
    {
        Normal,
        WideAngle
    }
}
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SixCloudCustomControlLibrary.Controls
{
    public class MetroWindow : Window
    {
        public static readonly DependencyProperty IsSubWindowShowProperty = DependencyProperty.Register("IsSubWindowShow", typeof(bool), typeof(MetroWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu", typeof(object), typeof(MetroWindow), new PropertyMetadata(null));
        public static new readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(MetroWindow));
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MetroWindow));

        public bool IsSubWindowShow { get => (bool)GetValue(IsSubWindowShowProperty); set => SetValue(IsSubWindowShowProperty, value); }
        public object Menu { get => GetValue(MenuProperty); set => SetValue(MenuProperty, value); }
        public new Brush BorderBrush { get => (Brush)GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
        public Brush TitleForeground { get => (Brush)GetValue(TitleForegroundProperty); set => SetValue(TitleForegroundProperty, value); }

        //private void BorderBrushChange(Brush brush)
        //{
        //    if (IsLoaded)
        //    {
        //        Theme.Switch(this);
        //    }
        //}

        //private void GoToState()
        //{
        //    ElementBase.GoToState(this, IsSubWindowShow ? "Enabled" : "Disable");
        //}

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
            // 修复WindowChrome导致的窗口大小错误
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
            //ElementBase.DefaultStyle<MetroWindow>(DefaultStyleKeyProperty);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }
    }
}
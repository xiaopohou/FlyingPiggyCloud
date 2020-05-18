using LibVLCSharp.Shared;
using SourceChord.FluentWPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace SixCloudCore.Views.VLCView
{
    /// <summary>
    /// VLCView.xaml 的交互逻辑
    /// </summary>
    public partial class VLCView : AcrylicWindow
    {
        private static readonly LibVLC libVlc = new LibVLC();
        private readonly MediaPlayer mediaPlayer = new MediaPlayer(libVlc);
        static VLCView()
        {
            Application.Current.Exit += (sender, e) =>
            {
                libVlc.Dispose();
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            mediaPlayer.Dispose();
        }

        public static readonly DependencyProperty FullScreenProperty = DependencyProperty.Register("FullScreenProperty", typeof(bool), typeof(PreView), new PropertyMetadata(false));
        public bool FullScreen
        {
            get => (bool)GetValue(FullScreenProperty);
            set
            {
                SetValue(FullScreenProperty, value);
                SetScreenStyle(value);
            }
        }




        public VLCView()
        {
            InitializeComponent();

            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            VideoViewer.Loaded += (sender, e) =>
            {
                VideoViewer.MediaPlayer = mediaPlayer;
                VideoViewer.MediaPlayer.EnableMouseInput = true;
                VideoViewer.MediaPlayer.Play(new Media(libVlc, "https://hls-source", FromType.FromLocation));
            };

            Activated += (sender, e) =>
            {

            };
        }

        protected void SetScreenStyle(bool isFullScreen)
        {
            if (isFullScreen)
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Topmost = true;
                Left = 0.0;
                Top = 0.0;
                Width = SystemParameters.PrimaryScreenWidth;
                Height = SystemParameters.PrimaryScreenHeight;
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                Topmost = false;
                Width = 800;
                Height = 450;
                Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
                Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
            }
        }

        private void ControllBar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ControllBar.BeginAnimation(HeightProperty, new DoubleAnimation
            {
                To = 120d,
                Duration = new Duration(TimeSpan.FromMilliseconds(150)),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut },
            });
            ControllBar.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                To = 0.98d,
                Duration = new Duration(TimeSpan.FromMilliseconds(150)),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut },
            });
        }

        private void ControllBar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            double height;
            double opacity;

            if (FullScreen)
            {
                height = 1d;
                opacity = 0.01d;
            }
            else
            {
                height = 20d;
                opacity = 0.1d;
            }

            ControllBar.BeginAnimation(HeightProperty, new DoubleAnimation
            {
                To = height,
                Duration = new Duration(TimeSpan.FromMilliseconds(150)),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseIn },
            });

            ControllBar.BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                To = opacity,
                Duration = new Duration(TimeSpan.FromMilliseconds(150)),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseIn },
            });


        }
    }
}

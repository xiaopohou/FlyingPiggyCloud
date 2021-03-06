﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace SixCloud.Core.Views.VLCView
{
    /// <summary>
    /// VLCView.xaml 的交互逻辑
    /// </summary>
    public partial class VLCView : Window
    {
        protected override void OnClosed(EventArgs e)
        {
            //mediaPlayer.Dispose();
            VideoViewer.MediaPlayer?.Stop();
        }


        public static readonly DependencyProperty FullScreenProperty = DependencyProperty.Register("FullScreenProperty", typeof(bool), typeof(VLCView), new PropertyMetadata(false));
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

        private void ProgressSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            (sender as Slider)?.GetBindingExpression(RangeBase.ValueProperty).UpdateSource();
        }
    }
}

using QingzhenyunApis.EntityModels;
using SixCloudCore.Controllers;
using SixCloudCore.Models;
using SixCloudCore.ViewModels;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SixCloudCore.Views
{
    /// <summary>
    /// PreView.xaml 的交互逻辑
    /// </summary>
    public partial class PreView : Window
    {
        public static readonly DependencyProperty FullScreenProperty = DependencyProperty.Register("FullScreenProperty", typeof(bool), typeof(PreView), new PropertyMetadata(false));
        public bool FullScreen { get => (bool)GetValue(FullScreenProperty); set => SetValue(FullScreenProperty, value); }

        //private object PreviewParameter { get; set; }

        public enum ResourceType
        {
            Text,
            Picture,
            Video
        }

        public ResourceType Type { get; set; }

        public PreView(ResourceType type, string uri, object parameter)
        {
            Type = type;
            //PreviewParameter = parameter;
            InitializeComponent();
            Loaded += (sender, e) =>
              {
                  switch (Type)
                  {
                      case ResourceType.Picture:
                          {
                              PreviewImageInformation p = parameter as PreviewImageInformation;
                              ImageContainer.Source = new BitmapImage(new Uri(uri));
                              VideoContainer.Visibility = Visibility.Collapsed;
                              BeginAnimation(WidthProperty, new DoubleAnimation(p.Width + 4, new Duration(TimeSpan.FromSeconds(1))));
                              BeginAnimation(HeightProperty, new DoubleAnimation(Width * p.Height / p.Width, new Duration(TimeSpan.FromSeconds(1))));
                              Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
                              Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
                              break;
                          }
                      case ResourceType.Video:
                          {
                              PreviewInformation p = parameter as PreviewInformation;
                              ImageContainer.Visibility = Visibility.Collapsed;
                              string address = uri;
                              string content = Properties.Resources.PreviewContainer.Replace("{{uri}}", address);
                              VideoContainer.NavigateToString(content);
                              VideoContainer.ContainsFullScreenElementChanged += (s, a) =>
                                {
                                    if (VideoContainer.ContainsFullScreenElement)
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
                                };
                              Closing += OnClose;
                              break;
                          }
                  }
              };
        }

        public void OnClose(object sender, EventArgs e)
        {
            VideoContainer.Close();
        }
    }
}

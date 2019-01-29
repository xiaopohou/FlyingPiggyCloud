using FlyingPiggyCloud.Controllers;
using FlyingPiggyCloud.Models;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// PreviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewWindow : Window
    {
        internal PreviewWindow(FileListItem e)
        {
            InitializeComponent();
            if (e.Preview == 1000)
            {
                var p = new PreviewVideo(e.UUID);
                p.LoadPreviewAddress(() =>
                {
                    string u = p.VideoSources.Preview[0]["url"] + "?token=" + p.CurrentToken;
                    string c = Properties.Resources.PreviewContainer.Replace("{{uri}}", u);
                    MainMedia.NavigateToString(c);
                });
            }
            else if (e.Preview == 300)
            {
                var p = new PreviewImage(e.UUID);
                p.LoadPreviewAddress(() =>
                {
                    MainImage.Source = new BitmapImage(new Uri(p.ImageSources.Address));
                });
            }
            else
            {
                throw new Exception("这不是一个支持预览的项目");
            }
            Title = e.Name;
            Closing += (sender,ags) => MainMedia.Close();
        }
    }
}

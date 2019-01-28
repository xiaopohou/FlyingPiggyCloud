using FlyingPiggyCloud.Controllers;
using FlyingPiggyCloud.Controllers.Results.FileSystem;
using FlyingPiggyCloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// PreviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewWindow : Window
    {
        internal PreviewWindow(FileListItem e)
        {
            Preview p;
            if(e.Preview==1000)
            {
                p = new PreviewVideo(e.UUID);
            }
            else
            {
                throw new Exception();
            }
            InitializeComponent();
            var a = p as PreviewVideo;
            MainMedia.Source = new Uri(a.VideoSources.Preview[0]["url"]);
            MainMedia.Play();
        }
    }
}

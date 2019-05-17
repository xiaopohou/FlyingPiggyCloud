using SixCloudCustomControlLibrary.Controls;
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

namespace SixCloud.Views
{
    /// <summary>
    /// OfflineUrlsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OfflineUrlsDialog : MetroWindow
    {
        public OfflineUrlsDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsWhichTypeStage.IsChecked ?? false)
            {
                IsInputUrlsStage.IsChecked = true;
            }
            else if (IsInputUrlsStage.IsChecked ?? false)
            {
                IsUploadTorrentStage.IsChecked = true;
            }
            else if (IsUploadTorrentStage.IsChecked ?? false)
            {
                IsCheckFilesStage.IsChecked = true;
            }
            else
            {
                IsWhichTypeStage.IsChecked = true;
            }
        }
    }
}

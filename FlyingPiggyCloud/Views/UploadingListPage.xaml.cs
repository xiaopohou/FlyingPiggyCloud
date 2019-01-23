using FlyingPiggyCloud.Controllers.Results.FileSystem;
using FlyingPiggyCloud.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlyingPiggyCloud.Views
{
    /// <summary>
    /// UploadingListPage.xaml 的交互逻辑
    /// </summary>
    public partial class UploadingListPage : Page
    {
        /// <summary>
        /// 上传列表，对该对象的写操作务必在主线程执行
        /// </summary>
        private static ObservableCollection<UploadTask> UploadTasks = new ObservableCollection<UploadTask>();

        public static async void NewUploadTask(UploadTask uploadTask,string parentUUID)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                lock (UploadTasks)
                {
                    UploadTasks.Add(uploadTask);
                }
            });
            await uploadTask.StartTaskAsync(parentUUID);
        }

        public static void NewUploadTask(Microsoft.Win32.OpenFileDialog openFileDialog, string parrentUUID)
        {
            NewUploadTask(new UploadTask(openFileDialog.FileName, openFileDialog.SafeFileName), parrentUUID);
        }

        public UploadingListPage()
        {
            InitializeComponent();
            UploadList.ItemsSource = UploadTasks;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var t = (UploadTask)s.DataContext;
            t.Cancel();
        }

        private void CancelAll_Click(object sender, RoutedEventArgs e)
        {
            foreach(UploadTask a in UploadTasks)
            {
                a.Cancel();
            }
            UploadTasks.Clear();
        }
    }
}

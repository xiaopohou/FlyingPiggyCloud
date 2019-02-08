﻿using FlyingPiggyCloud.Controllers.Results.FileSystem;
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
        public static string LabelName => string.Format($"正在上传 （{UploadTasks.Count}）");

        public static event EventHandler OnListCountChanged;

        /// <summary>
        /// 上传列表，对该对象的写操作务必在主线程执行
        /// </summary>
        private static ObservableCollection<IUploadTask> UploadTasks = new ObservableCollection<IUploadTask>();

        public static async void NewUploadTask(SingleFileUploadTask uploadTask,string parentUUID)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                lock (UploadTasks)
                {
                    UploadTasks.Add(uploadTask);
                    OnListCountChanged?.Invoke(UploadTasks, new EventArgs());
                }
            });
            await uploadTask.StartTaskAsync(parentUUID);
        }

        public static void NewUploadTask(Microsoft.Win32.OpenFileDialog openFileDialog, string parrentUUID)
        {
            foreach(string path in openFileDialog.FileNames)
            {
                NewUploadTask(new SingleFileUploadTask(path, openFileDialog.SafeFileName), parrentUUID);
            }
        }

        public UploadingListPage()
        {
            InitializeComponent();
            UploadList.ItemsSource = UploadTasks;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var s = (Button)sender;
            var t = (IUploadTask)s.DataContext;
            t.Cancel();
        }

        private void CancelAll_Click(object sender, RoutedEventArgs e)
        {
            foreach(IUploadTask a in UploadTasks)
            {
                a.Cancel();
            }
            UploadTasks.Clear();
        }
    }
}

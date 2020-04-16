using SixCloudCore.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SixCloudCore.Views.UserControls
{
    /// <summary>
    /// UploadingListView.xaml 的交互逻辑
    /// </summary>
    public partial class UploadingListView : UserControl
    {
        public UploadingListView()
        {
            InitializeComponent();
        }

        private void UploadingTaskPause(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<UploadingTaskViewModel> downloadingTasks = list.Cast<UploadingTaskViewModel>();
                foreach (UploadingTaskViewModel t in downloadingTasks)
                {
                    if (t.Status == UploadingTaskViewModel.UploadStatus.Pause)
                    {
                        t.Start();
                    }
                    else if (t.Status == UploadingTaskViewModel.UploadStatus.Running)
                    {
                        t.Pause();
                    }
                }

            }
            catch (Exception)
            {
                return;
            }
        }

        private void UploadingTaskCancel(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<UploadingTaskViewModel> downloadingTasks = list.Cast<UploadingTaskViewModel>();
                foreach (var t in downloadingTasks)
                {
                    t.Stop(null);
                }

            }
            catch (Exception)
            {

            }
        }


    }
}

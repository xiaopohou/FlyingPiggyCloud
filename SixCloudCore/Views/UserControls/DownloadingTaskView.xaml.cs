using SixCloudCore.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace SixCloudCore.Views.UserControls
{
    /// <summary>
    /// DownloadingTaskView.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadingTaskView : UserControl
    {
        public DownloadingTaskView()
        {
            InitializeComponent();
        }

        private void DownloadingTaskPause(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<DownloadingTaskViewModel> downloadingTasks = list.Cast<DownloadingTaskViewModel>();
                foreach (DownloadingTaskViewModel t in downloadingTasks)
                {
                    if (t.Status == TransferTaskStatus.Pause)
                    {
                        t.RecoveryCommand.Execute(null);
                    }
                    else if (t.Status == TransferTaskStatus.Running)
                    {
                        t.PauseCommand.Execute(null);
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        private void DownloadingTaskCancel(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<DownloadingTaskViewModel> downloadingTasks = list.Cast<DownloadingTaskViewModel>();
                foreach (DownloadingTaskViewModel t in downloadingTasks.ToArray())
                {
                    t.CancelCommand.Execute(null);
                }

            }
            catch (Exception)
            {

            }
        }


    }

}

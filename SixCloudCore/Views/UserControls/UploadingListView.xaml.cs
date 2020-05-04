﻿using SixCloudCore.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

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
                return;
            }
        }

        private void UploadingTaskCancel(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IList list = (IList)e.Parameter;
                IEnumerable<UploadingTaskViewModel> downloadingTasks = list.Cast<UploadingTaskViewModel>();
                foreach (UploadingTaskViewModel t in downloadingTasks)
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

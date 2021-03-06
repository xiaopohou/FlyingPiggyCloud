﻿using SixCloud.Core.ViewModels;
using System;
using System.Collections;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace SixCloud.Core.Views.UserControls
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
                var list = (IList)e.Parameter;
                var downloadingTasks = list.Cast<UploadingTaskViewModel>();
                foreach (var t in downloadingTasks)
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
                var list = (IList)e.Parameter;
                var downloadingTasks = list.Cast<UploadingTaskViewModel>();
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

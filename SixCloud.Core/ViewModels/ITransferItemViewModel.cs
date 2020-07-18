﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace SixCloud.Core.ViewModels
{
    public interface ITransferItemViewModel
    {

        /// <summary>
        /// 用于定时刷新任务进度
        /// </summary>
        protected static readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
        {
            Interval = TimeSpan.FromSeconds(0.5d)
        };

        /// <summary>
        /// 全局任务列表，修改此集合需加锁
        /// </summary>
        protected static readonly ObservableCollection<ITransferItemViewModel> taskList = new ObservableCollection<ITransferItemViewModel>();

        public string Icon { get; }

        public string Name { get; }

        public double Progress { get; }

        public TransferTaskStatus Status { get; }

        public string Completed { get; }

        public string Total { get; }

        public string FriendlySpeed { get; }

        public DependencyCommand RecoveryCommand { get; }

        public DependencyCommand PauseCommand { get; }

        public DependencyCommand CancelCommand { get; }

        static ITransferItemViewModel()
        {
            timer.Start();
        }
    }
}

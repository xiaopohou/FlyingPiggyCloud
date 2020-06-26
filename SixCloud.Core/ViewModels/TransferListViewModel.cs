﻿using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloud.Core.Controllers;
using SixCloud.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.Core.ViewModels
{
    public class TransferListViewModel : ViewModelBase
    {
        public string DownloadingListTitle => DownloadingList.Count == 0 ? string.Empty : $"在下载队列中（{DownloadingList.Count}）";

        public Visibility DownloadingListVisibility => DownloadingListTitle == string.Empty ? Visibility.Collapsed : Visibility.Visible;

        public string UploadingListTitle => UploadingList.Count == 0 ? string.Empty : $"在上传队列中（{UploadingList.Count}）";

        public Visibility UploadingListVisibility => UploadingListTitle == string.Empty ? Visibility.Collapsed : Visibility.Visible;


        public ObservableCollection<DownloadingTaskViewModel> DownloadingList => downloadingList;

        public ObservableCollection<UploadingTaskViewModel> UploadingList => uploadingList;

        public TransferCompletedListViewModel TransferCompletedList { get; private set; } = new TransferCompletedListViewModel();

        #region FromDownloadingListViewModel

        protected static readonly ObservableCollection<DownloadingTaskViewModel> downloadingList = new ObservableCollection<DownloadingTaskViewModel>();

        /// <summary>
        /// 创建新的下载任务
        /// </summary>
        /// <param name="targetUUID">下载对象的uuid</param>
        /// <param name="downloadAddress">下载地址</param>
        /// <param name="localPath">本地路径</param>
        /// <param name="name">文件名</param>
        /// <param name="isAutoStart">自动开始</param>
        public static async Task NewDownloadTask(string targetUUID, string localPath, string name, bool isAutoStart = true)
        {
            var detail = await FileSystem.GetDetailsByIdentity(targetUUID);
            DownloadingTaskViewModel task;

            if (detail.Size == 0)
            {
                task = new EmptyFileDownloadTask(localPath, name, targetUUID);
            }
            else
            {
                task = new DownloadTask(localPath, name, targetUUID);

            }

            //当下载任务结束时从列表中移除任务信息
            task.DownloadCompleted += (sender, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            downloadingList.Remove(task);
                            if (File.Exists(task.CurrentFileFullPath))
                            {
                                TransferCompletedListViewModel.NewDownloadedTask(task);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.ToSentry().TreatedBy("DownloadCompletedEventHandler").Submit();
                        }
                    });
                };

            task.DownloadCanceled += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        downloadingList.Remove(task);
                    }
                    catch (Exception ex)
                    {
                        ex.ToSentry().TreatedBy("DownloadCompletedEventHandler").Submit();
                    }
                });
            };

            AddDownloadingItem(isAutoStart, task);

        }

        public static async void NewDownloadTaskGroup(string targetUUID, string localPath, string name, bool isAutoStart = true)
        {
            try
            {
                DownloadTaskGroup task = new DownloadTaskGroup(targetUUID, localPath, name);
                AddDownloadingItem(false, task);
                await task.InitTaskGroup();
                task.DownloadCompleted += (sender, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        downloadingList.Remove(task);
                        TransferCompletedListViewModel.NewDownloadedTask(task);
                    });
                };

                task.DownloadCanceled += (sender, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            downloadingList.Remove(task);
                        }
                        catch (Exception ex)
                        {
                            ex.Submit("DownloadCompletedEventHandler");
                        }
                    });
                };

                if (isAutoStart)
                {
                    task.RecoveryCommand.Execute(null);
                }

            }
            catch (IOException ex) when (ex.Message.Contains("不正确"))
            {
                MessageBox.Show(ex.Message, "失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        public static void NewDownloadTaskGroup(DownloadTaskGroupRecord record, bool isAutoStart = true)
        {
            DownloadingTaskViewModel task = new DownloadTaskGroup(record);


            task.DownloadCompleted += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    downloadingList.Remove(task);
                    TransferCompletedListViewModel.NewDownloadedTask(task);
                });
            };

            task.DownloadCanceled += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        downloadingList.Remove(task);
                    }
                    catch (Exception ex)
                    {
                        ex.ToSentry().TreatedBy("DownloadCompletedEventHandler").Submit();
                    }
                });
            };

            AddDownloadingItem(isAutoStart, task);

        }


        private static void AddDownloadingItem(bool isAutoStart, DownloadingTaskViewModel task)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                downloadingList.Add(task);
            });
            if (isAutoStart)
            {
                task.RecoveryCommand.Execute(null);
            }
        }



        #endregion

        #region FromUploadingListViewModel
        protected static readonly ObservableCollection<UploadingTaskViewModel> uploadingList = new ObservableCollection<UploadingTaskViewModel>();

        public static void NewUploadTask(FileListViewModel targetList, string path)
        {
            NewUploadTask(targetList.CurrentPath, path);
        }

        public static async void NewUploadTask(string targetPath, string path)
        {
            if (Directory.Exists(path))
            {

            }

            else if (File.Exists(path))
            {
                UploadingFileViewModel task = new UploadingFileViewModel(targetPath, path);
                task.UploadCompleted += CompletedEventHandler;
                task.UploadAborted += AbortedEventHandler;
                await task.Run();
                Application.Current.Dispatcher.Invoke(() => uploadingList.Add(task));

                void CompletedEventHandler(object sender, EventArgs e)
                {
                    task.UploadCompleted -= CompletedEventHandler;
                    uploadingList.Remove(task);
                    TransferCompletedListViewModel.NewUploadedTask(task);
                };
                void AbortedEventHandler(object sender, EventArgs e)
                {
                    task.UploadAborted -= AbortedEventHandler;
                    uploadingList.Remove(task);
                };
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show("由于找不到对象，6盘未能创建任务", "失败", MessageBoxButton.OK, MessageBoxImage.Stop));
            }
        }


        #endregion

        static TransferListViewModel()
        {
            TasksLogger.Downloadings = downloadingList;
            TasksLogger.Uploadings = uploadingList;
        }

        public TransferListViewModel()
        {
            WeakEventManager<ObservableCollection<DownloadingTaskViewModel>, NotifyCollectionChangedEventArgs>
                .AddHandler(downloadingList, nameof(downloadingList.CollectionChanged), (sender, e) =>
                {
                    OnPropertyChanged(nameof(DownloadingListTitle));
                    OnPropertyChanged(nameof(DownloadingListVisibility));
                });
            WeakEventManager<ObservableCollection<UploadingTaskViewModel>, NotifyCollectionChangedEventArgs>
                .AddHandler(uploadingList, nameof(uploadingList.CollectionChanged), (sender, e) =>
                {
                    OnPropertyChanged(nameof(UploadingListTitle));
                    OnPropertyChanged(nameof(UploadingListVisibility));
                });
        }
    }
}

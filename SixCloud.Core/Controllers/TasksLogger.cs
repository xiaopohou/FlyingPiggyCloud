using Newtonsoft.Json;
using QingzhenyunApis.EntityModels;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloud.Core.Models;
using SixCloud.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace SixCloud.Core.Controllers
{
    internal partial class TasksLogger
    {
        private static readonly string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SixCloud";
        private static readonly string uploadingRecordsPath = rootDirectory + "/UploadRecord.json";
        //在v3.0.4及以前版本使用，将在未来移除
        private static readonly string downloadingRecordsPath = rootDirectory + "/DownloadRecord.json";
        //在v3.0.5版本引入，将上传、下载配置信息整合至同一文件中
        private static readonly string startupInformationPath = rootDirectory + "/StartupInformation_3.0.5.json";


        public static ObservableCollection<UploadingTaskViewModel> Uploadings
        {
            set => uploadingList ??= value;
        }
        private static ObservableCollection<UploadingTaskViewModel> uploadingList;

        public static ObservableCollection<DownloadingTaskViewModel> Downloadings
        {
            set => downloadingList ??= value;
        }
        private static ObservableCollection<DownloadingTaskViewModel> downloadingList;

        private class StartupInformation
        {
            public UserInformation BelongsTo { get; set; }

            public IList<UploadTaskRecord> UploadTasks { get; set; }

            public IList<string> SerializedDownloadTasks { get; set; }

            public IList<string> SerializedDownloadTaskGroups { get; set; }
        }

        public static void StartUpRecovery(UserInformation user)
        {
            try
            {
                if (File.Exists(startupInformationPath))
                {
                    string s = File.ReadAllText(startupInformationPath);
                    StartupInformation startupInformation = JsonConvert.DeserializeObject<StartupInformation>(s);

                    if (startupInformation?.BelongsTo?.UUID == user.UUID)
                    {
                        if (startupInformation?.UploadTasks?.Any() == true)
                        {
                            foreach (UploadTaskRecord record in startupInformation.UploadTasks)
                            {
                                try
                                {
                                    Application.Current.Dispatcher.Invoke(() => TransferListViewModel.NewUploadTask(record.TargetPath, record.LocalFilePath));
                                }
                                catch (NullReferenceException ex)
                                {
                                    ex
                                        .ToSentry()
                                        .AttachTag("Location", nameof(UploadTaskRecord))
                                        .AttachExtraInfo(nameof(record), record)
                                        .Submit();
                                }
                            }
                        }

                        if (startupInformation?.SerializedDownloadTasks?.Any() == true)
                        {
                            foreach (string record in startupInformation.SerializedDownloadTasks)
                            {
                                try
                                {
                                    DownloadTaskRecord downloadTaskRecord = JsonConvert.DeserializeObject<DownloadTaskRecord>(record);
                                    DownloadingListViewModel.NewTask(downloadTaskRecord.TargetUUID, downloadTaskRecord.LocalPath, downloadTaskRecord.Name);
                                }
                                catch (NullReferenceException ex)
                                {
                                    ex
                                        .ToSentry()
                                        .AttachTag("Location", nameof(DownloadTaskRecord))
                                        .AttachExtraInfo(nameof(record), record)
                                        .AttachExtraInfo(nameof(DownloadTaskRecord), JsonConvert.DeserializeObject<DownloadTaskRecord>(record))
                                        .Submit();
                                }

                            }
                        }

                        if (startupInformation?.SerializedDownloadTaskGroups?.Any() == true)
                        {
                            foreach (string record in startupInformation.SerializedDownloadTaskGroups)
                            {
                                try
                                {
                                    DownloadTaskGroupRecord downloadTaskGroupRecord = JsonConvert.DeserializeObject<DownloadTaskGroupRecord>(record);
                                    TransferListViewModel.NewDownloadTaskGroup(downloadTaskGroupRecord);
                                }
                                catch (NullReferenceException ex)
                                {
                                    ex
                                        .ToSentry()
                                        .AttachTag("Location", nameof(DownloadTaskGroupRecord))
                                        .AttachExtraInfo(nameof(record), record)
                                        .AttachExtraInfo(nameof(DownloadTaskGroupRecord), JsonConvert.DeserializeObject<DownloadTaskGroupRecord>(record))
                                        .Submit();
                                }
                            }
                        }
                    }

                    return;
                }

                //在v3.0.5版本解析该文件并移除，在未来将弃用该文件
                if (File.Exists(uploadingRecordsPath))
                {
                    string s = File.ReadAllText(uploadingRecordsPath);
                    UploadTaskRecord[] list = JsonConvert.DeserializeObject<UploadTaskRecord[]>(s);
                    if (list != null && list.Length > 0)
                    {
                        foreach (UploadTaskRecord record in list)
                        {
                            Application.Current.Dispatcher.Invoke(() => TransferListViewModel.NewUploadTask(record.TargetPath, record.LocalFilePath));
                        }
                    }
                }

                //在v3.0.5版本解析该文件并移除，在未来将弃用该文件
                if (File.Exists(downloadingRecordsPath))
                {
                    string s = File.ReadAllText(downloadingRecordsPath);
                    DownloadTaskRecord[] list = JsonConvert.DeserializeObject<DownloadTaskRecord[]>(s);
                    if (list != null && list.Length > 0)
                    {
                        foreach (DownloadTaskRecord record in list)
                        {
                            DownloadingListViewModel.NewTask(record.TargetUUID, record.LocalPath, record.Name);
                        }
                    }
                }


            }
            catch (InvalidOperationException ex)
            {
                ex.Submit();
            }
        }

        static TasksLogger()
        {
            Directory.CreateDirectory(rootDirectory);
        }

        public static void ExitEventHandler(object sender, ExitEventArgs e)
        {
            StartupInformation startupInformation = new StartupInformation
            {
                BelongsTo = e.CurrentUser
            };

            IEnumerable<IGrouping<bool, string>> downloadLists = from record in downloadingList
                                                                 where record.Status == TransferTaskStatus.Running || record.Status == TransferTaskStatus.Pause || record.Status == TransferTaskStatus.Failed
                                                                 group record.ToString() by record is DownloadTask;
            startupInformation.SerializedDownloadTasks = downloadLists.FirstOrDefault(x => x.Key == true)?.ToArray();
            startupInformation.SerializedDownloadTaskGroups = downloadLists.FirstOrDefault(x => x.Key == false)?.ToArray();

            IEnumerable<UploadTaskRecord> uploadList = from record in uploadingList
                                                       where record.Status == TransferTaskStatus.Running || record.Status == TransferTaskStatus.Pause || record.Status == TransferTaskStatus.Failed
                                                       select new UploadTaskRecord
                                                       {
                                                           LocalFilePath = record.LocalFilePath,
                                                           TargetPath = record.TargetPath
                                                       };
            startupInformation.UploadTasks = uploadList.ToArray();

            using StreamWriter writer = new StreamWriter(File.Create(startupInformationPath));
            writer.Write(JsonConvert.SerializeObject(startupInformation));

            LocalProperties.Token = SixCloudMethodBase.Token ?? string.Empty;
        }
    }


    internal class ExitEventArgs : EventArgs
    {
        public UserInformation CurrentUser { get; }

        public ExitEventArgs(UserInformation currentUser)
        {
            CurrentUser = currentUser;
        }
    }
}
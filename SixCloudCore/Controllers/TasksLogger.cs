using Newtonsoft.Json;
using QingzhenyunApis.Exceptions;
using QingzhenyunApis.Methods.V3;
using SixCloudCore.Models;
using SixCloudCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SixCloudCore.Controllers
{
    internal partial class TasksLogger
    {
        private static readonly string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SixCloud";
        private static readonly string uploadingRecordsPath = rootDirectory + "/UploadRecord.json";
        //在v3.0.4及以前版本使用，将在未来移除
        private static readonly string downloadingRecordsPath = rootDirectory + "/DownloadRecord.json";
        //在v3.0.5版本引入，除downloadTask外，还记录downloadTaskGroup
        private static readonly string downloadingGroupRecordsPath = rootDirectory + "/DownloadGroupRecord.json";


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

        public static void StartUpRecovery()
        {
            try
            {
                if (File.Exists(uploadingRecordsPath))
                {
                    string s = File.ReadAllText(uploadingRecordsPath);
                    UploadTaskRecord[] list = JsonConvert.DeserializeObject<UploadTaskRecord[]>(s);
                    if (list != null && list.Length > 0)
                    {
                        foreach (UploadTaskRecord record in list)
                        {
                            App.Current.Dispatcher.Invoke(() => TransferListViewModel.NewUploadTask(record.TargetPath, record.LocalFilePath));
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


                if (File.Exists(downloadingGroupRecordsPath))
                {
                    string s = File.ReadAllText(downloadingRecordsPath);
                    dynamic[] list = JsonConvert.DeserializeObject<dynamic[]>(s);
                    if (list != null && list.Length > 0)
                    {
                        foreach (var record in list)
                        {
                            if (record.Type == nameof(DownloadTask))
                            {
                                var downloadTaskRecord = JsonConvert.DeserializeObject<DownloadTaskRecord>(record.Record);
                                DownloadingListViewModel.NewTask(downloadTaskRecord.TargetUUID, downloadTaskRecord.LocalPath, downloadTaskRecord.Name);
                            }
                            else
                            {
                                var downloadTaskGroupRecord = JsonConvert.DeserializeObject<DownloadTaskGroupRecord>(record.Record);
                                TransferListViewModel.NewDownloadTaskGroup(downloadTaskGroupRecord);
                            }
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

        public static void ExitEventHandler(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(File.Create(downloadingGroupRecordsPath)))
            {
                var taskList = from record in downloadingList
                               where record.Status == TransferTaskStatus.Running || record.Status == TransferTaskStatus.Pause
                               select new { Type = record is DownloadTask ? nameof(DownloadTask) : nameof(DownloadTaskGroup), Record = record.ToString() };


                //taskList.ToList().ForEach(task => task.PauseCommand.Execute(null));

                //string s = JsonConvert.SerializeObject(taskList.Select(task =>
                //{
                //    return new DownloadTaskRecord
                //    {
                //        LocalPath = task.SavedLocalPath,
                //        TargetUUID = task.TargetUUID,
                //        Name = task.Name,
                //    };
                //}));

                writer.Write(JsonConvert.SerializeObject(taskList));
            }

            using (StreamWriter writer = new StreamWriter(File.Create(uploadingRecordsPath)))
            {
                IEnumerable<UploadTaskRecord> taskList = from record in uploadingList
                                                         where record.Status == TransferTaskStatus.Running || record.Status == TransferTaskStatus.Pause
                                                         select new UploadTaskRecord
                                                         {
                                                             LocalFilePath = record.LocalFilePath,
                                                             TargetPath = record.TargetPath
                                                         };
                string s = JsonConvert.SerializeObject(taskList);
                writer.Write(s);
            }

            LocalProperties.Token = SixCloudMethodBase.Token ?? string.Empty;
        }
    }
}
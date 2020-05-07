//#define Record
//using Exceptionless;
using Newtonsoft.Json;
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
        private static readonly string downloadingRecordsPath = rootDirectory + "/DownloadRecord.json";

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

        public static void ExitEventHandler(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(File.Create(downloadingRecordsPath)))
            {
                IEnumerable<DownloadTaskRecord> taskList = from record in downloadingList
                                                           where record.Status == TransferTaskStatus.Running || record.Status == TransferTaskStatus.Pause
                                                           select new DownloadTaskRecord
                                                           {
                                                               LocalPath = record.SavedLocalPath,
                                                               TargetUUID = record.TargetUUID,
                                                               Name = record.Name,
                                                           };
                string s = JsonConvert.SerializeObject(taskList);
                writer.Write(s);
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
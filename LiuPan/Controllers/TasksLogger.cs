//#define Record
using FileDownloader;
using Newtonsoft.Json;
using SixCloud.Models;
using SixCloud.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using Exceptionless;
using QingzhenyunApis.Methods;

namespace SixCloud.Controllers
{
    internal class TasksLogger : IDownloadCache
    {
        private static readonly string rootDirectory;
        private static readonly string uploadingRecordsPath;
        private static readonly string downloadingRecordsPath;
        private static readonly string downloadLoggerFilePath;

        /// <summary>
        /// 这个字典用于服务FileDownloader IDownloadCache
        /// </summary>
        private static readonly Dictionary<string, string> downloadCache;

        public static ObservableCollection<UploadingTaskViewModel> Uploadings
        {
            set
            {
                uploadingList = uploadingList ?? value;
            }
        }
        private static ObservableCollection<UploadingTaskViewModel> uploadingList;

        public static ObservableCollection<DownloadingTaskViewModel> Downloadings
        {
            set
            {
                downloadingList = downloadingList ?? value;
            }
        }
        private static ObservableCollection<DownloadingTaskViewModel> downloadingList;

        public static void StartUpRecovery()
        {
            try
            {
                if (File.Exists(uploadingRecordsPath))
                {
                    var s = File.ReadAllText(uploadingRecordsPath);
                    var list = JsonConvert.DeserializeObject<UploadTaskRecord[]>(s);
                    if (list != null && list.Length > 0)
                    {
                        foreach (var record in list)
                        {
                            App.Current.Dispatcher.Invoke(async () => await UploadingListViewModel.NewTask(record.TargetPath, record.LocalFilePath));
                        }
                    }
                }

                if (File.Exists(downloadingRecordsPath))
                {
                    FileSystem fs = new FileSystem();
                    var s = File.ReadAllText(downloadingRecordsPath);
                    var list = JsonConvert.DeserializeObject<DownloadTaskRecord[]>(s);
                    if (list != null && list.Length > 0)
                    {
                        foreach (var record in list)
                        {
                            var result = fs.GetDetailsByUUID(record.TargetUUID);
                            DownloadingListViewModel.NewTask(record.TargetUUID, result.Result.Result.DownloadAddress, record.LocalPath, record.Name);
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                ex.ToExceptionless();
            }
        }

        static TasksLogger()
        {
            rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SixCloud";
            Directory.CreateDirectory(rootDirectory);
            uploadingRecordsPath = rootDirectory + "/UploadRecord.json";
            downloadLoggerFilePath = rootDirectory + "/DownloadTasksLogger.json";
            downloadingRecordsPath = rootDirectory + "/DownloadRecord.json";

            #region downloadLogger
            if (File.Exists(downloadLoggerFilePath))
            {
                string s = File.ReadAllText(downloadLoggerFilePath);
                downloadCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(s) ?? new Dictionary<string, string>();
            }
            else
            {
                File.Create(downloadLoggerFilePath).Close();
                downloadCache = new Dictionary<string, string>();
            }
            #endregion
#if Record
            #region record
            //用于处理旧的记录文件，在两个版本后移除
            var recordFilePath = rootDirectory + "/Records.json";
            if (File.Exists(recordFilePath))
            {
                string s = File.ReadAllText(recordFilePath);
                var records = JsonConvert.DeserializeObject<List<Record>>(s) ?? new List<Record>();
                File.Delete(recordFilePath);
                if (records.Count != 0)
                {
                    foreach (Record a in records)
                    {
                        DownloadingListViewModel.NewTask(null, a.DownloadAddress, a.Path, a.Name);
                    }
                }
            }
            #endregion
#endif
        }

        public static void ExitEventHandler(object sender, EventArgs e)
        {
            if (downloadCache != null)
            {
                using (StreamWriter writer = new StreamWriter(File.Create(downloadLoggerFilePath)))
                {
                    string s = JsonConvert.SerializeObject(downloadCache);
                    writer.Write(s);
                }

            }

            using (StreamWriter writer = new StreamWriter(File.Create(downloadingRecordsPath)))
            {
                var taskList = from record in downloadingList
                               where record.Status == DownloadTask.TaskStatus.Running || record.Status == DownloadTask.TaskStatus.Pause
                               select new DownloadTaskRecord
                               {
                                   LocalPath = record.SavedLocalPath,
                                   TargetUUID = record.TargetUUID,
                                   DownloadAddress = record.DownloadAddress,
                                   Name = record.Name
                               };
                string s = JsonConvert.SerializeObject(taskList);
                writer.Write(s);
            }

            using (StreamWriter writer = new StreamWriter(File.Create(uploadingRecordsPath)))
            {
                var taskList = from record in uploadingList
                               where record.Status == UploadingTaskViewModel.UploadStatus.Running || record.Status == UploadingTaskViewModel.UploadStatus.Pause
                               select new UploadTaskRecord
                               {
                                   LocalFilePath = record.LocalFilePath,
                                   TargetPath = record.TargetPath
                               };
                string s = JsonConvert.SerializeObject(taskList);
                writer.Write(s);
            }
        }


        #region 这三个方法用于FileDownloader下载任务恢复
        public void Add(Uri uri, string path, WebHeaderCollection headers)
        {
            lock (downloadCache)
            {
                downloadCache[uri.ToString()] = path;
            }
        }

        public string Get(Uri uri, WebHeaderCollection headers)
        {
            lock (downloadCache)
            {
                if (!downloadCache.TryGetValue(uri.ToString(), out string s))
                {
                    s = null;
                }
                return s;
            }
        }

        public void Invalidate(Uri uri)
        {
            lock (downloadCache)
            {
                downloadCache.Remove(uri.ToString());
            }
        }
        #endregion

#if Record
        private class Record
        {
            public string DownloadAddress { get; set; }

            public string Path { get; set; }

            public string Name { get; set; }

            public Record(DownloadTask task)
            {
                DownloadAddress = task.DownloadAddress;
                Path = task.Path;
                Name = task.Name;
            }

            public Record()
            {

            }
        }
#endif
        private class DownloadTaskRecord
        {
            public string LocalPath { get; set; }

            public string TargetUUID { get; set; }

            public string DownloadAddress { get; set; }

            public string Name { get; set; }

        }

        private class UploadTaskRecord
        {
            public string LocalFilePath { get; set; }

            public string TargetPath { get; set; }
        }
    }
}
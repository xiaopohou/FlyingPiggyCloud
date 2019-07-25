using FileDownloader;
using Newtonsoft.Json;
using SixCloud.Models;
using SixCloud.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.Controllers
{
    internal class TasksLogger : IDownloadCache
    {
        private static readonly Dictionary<string, string> taskList;

        private static readonly List<DownloadTaskRecord> records;

        private static ObservableCollection<UploadingTaskViewModel> uploadingTaskListPrinter;

        public static void StartUpRecovery()
        {
            if (taskList.Count != 0)
            {
                foreach (DownloadTaskRecord a in records)
                {
                    DownloadingListViewModel.NewTask(a.DownloadAddress, a.Path, a.Name);
                }
            }
        }

        public static void AddRecord(DownloadTask task)
        {
            DownloadTaskRecord record = new DownloadTaskRecord(task);
            task.DownloadFileCompleted += (sender, e) =>
            {
                Task.Run(() =>
                {
                    records.Remove(record);
                });
            };
            foreach (DownloadTaskRecord a in records)
            {
                if (a.DownloadAddress == task.DownloadAddress || a.Path == task.Path)
                {
                    return;
                }
            }
            records.Add(record);
        }

        internal static void SetUploadingTaskList(ObservableCollection<UploadingTaskViewModel> printer)
        {
            lock (uploadingTaskListPrinter)
            {
                if (uploadingTaskListPrinter == null)
                {
                    uploadingTaskListPrinter = printer;
                }
            }
        }

        static TasksLogger()
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SixCloud";
            Directory.CreateDirectory(rootDirectory);

            #region downloadLogger
            string downloadLoggerFilePath = rootDirectory + "/DownloadTasksLogger.json";
            if (File.Exists(downloadLoggerFilePath))
            {
                string s = File.ReadAllText(downloadLoggerFilePath);
                taskList = JsonConvert.DeserializeObject<Dictionary<string, string>>(s) ?? new Dictionary<string, string>();
            }
            else
            {
                File.Create(downloadLoggerFilePath).Close();
                taskList = new Dictionary<string, string>();
            }
            #endregion

            #region record
            string recordFilePath = rootDirectory + "/Records.json";
            if (File.Exists(recordFilePath))
            {
                string s = File.ReadAllText(recordFilePath);
                records = JsonConvert.DeserializeObject<List<DownloadTaskRecord>>(s) ?? new List<DownloadTaskRecord>();
            }
            else
            {
                File.Create(recordFilePath).Close();
                records = new List<DownloadTaskRecord>();
            }
            #endregion

            #region uploadLogger
            string uploadLoggerFilePath = rootDirectory + "/UploadTasksLogger.json";
            if (File.Exists(uploadLoggerFilePath))
            {
                string s = File.ReadAllText(uploadLoggerFilePath);
                string[] uploadTasks = JsonConvert.DeserializeObject<string[]>(s);
                File.Delete(uploadLoggerFilePath);
                //UploadingListViewModel.NewTask(new )
#warning 这里的上传恢复代码没有完成
            }
            #endregion

            Application.Current.Exit += (sender, e) =>
            {
                using (StreamWriter writer = new StreamWriter(File.Create(downloadLoggerFilePath)))
                {
                    string s = JsonConvert.SerializeObject(taskList);
                    writer.Write(s);
                }
                using (StreamWriter writer = new StreamWriter(File.Create(recordFilePath)))
                {
                    string s = JsonConvert.SerializeObject(records);
                    writer.Write(s);
                }

                //if (uploadingTaskListPrinter != null)
                //{
                //    List<string> recordList = new List<string>(uploadingTaskListPrinter.Count);
                //    lock (uploadingTaskListPrinter)
                //    {
                //        foreach (UploadingTaskViewModel a in uploadingTaskListPrinter)
                //        {
                //            if (a is UploadingFileViewModel task)
                //            {
                //                recordList.Add(task.ToString());
                //            }
                //        }
                //    }

                //}
            };
        }

        public void Add(Uri uri, string path, WebHeaderCollection headers)
        {
            lock (taskList)
            {
                taskList[uri.ToString()] = path;
            }
        }

        public string Get(Uri uri, WebHeaderCollection headers)
        {
            lock (taskList)
            {
                if (!taskList.TryGetValue(uri.ToString(), out string s))
                {
                    s = null;
                }
                return s;
            }
        }

        public void Invalidate(Uri uri)
        {
            lock (taskList)
            {
                taskList.Remove(uri.ToString());
            }
        }

        private class DownloadTaskRecord
        {
            public string DownloadAddress { get; set; }

            public string Path { get; set; }

            public string Name { get; set; }

            public DownloadTaskRecord(DownloadTask task)
            {
                DownloadAddress = task.DownloadAddress;
                Path = task.Path;
                Name = task.Name;
            }
        }
    }
}
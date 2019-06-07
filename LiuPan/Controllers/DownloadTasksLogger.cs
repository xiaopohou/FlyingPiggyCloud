using FileDownloader;
using Newtonsoft.Json;
using SixCloud.Models;
using SixCloud.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace SixCloud.Controllers
{
    internal class DownloadTasksLogger : IDownloadCache
    {
        private static readonly Dictionary<string, string> taskList;

        private static readonly List<DownloadTask> records;

        public static void StartUpRecovery()
        {
            if (taskList.Count != 0)
            {
                foreach (DownloadTask a in records)
                {
                    DownloadingListViewModel.NewTask(a.DownloadAddress, a.Path, a.Name);
                }
            }
        }

        public static void AddRecord(DownloadTask task)
        {

            task.DownloadFileCompleted += (sender, e) =>
            {
                Task.Run(() =>
                {
                    records.Remove(task);
                });
            };
            foreach (DownloadTask a in records)
            {
                if (a.DownloadAddress == task.DownloadAddress || a.Path == task.Path)
                {
                    return;
                }
            }
            records.Add(task);
        }

        static DownloadTasksLogger()
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SixCloud";
            if (!Directory.Exists(rootDirectory))
            {
                Directory.CreateDirectory(rootDirectory);
            }

            string loggerFilePath = rootDirectory + "/DownloadTasksLogger.json";
            if (File.Exists(loggerFilePath))
            {
                string s = File.ReadAllText(loggerFilePath);
                taskList = JsonConvert.DeserializeObject<Dictionary<string, string>>(s) ?? new Dictionary<string, string>();
            }
            else
            {
                File.Create(loggerFilePath).Close();
                taskList = new Dictionary<string, string>();
            }

            string recordFilePath = rootDirectory + "/Records.json";
            if (File.Exists(recordFilePath))
            {
                string s = File.ReadAllText(recordFilePath);
                records = JsonConvert.DeserializeObject<List<DownloadTask>>(s) ?? new List<DownloadTask>();
            }
            else
            {
                File.Create(recordFilePath).Close();
                records = new List<DownloadTask>();
            }
            Application.Current.Exit += (sender, e) =>
            {
                new StreamWriter(File.Create(loggerFilePath)).Write(JsonConvert.SerializeObject(taskList));
                new StreamWriter(File.Create(recordFilePath)).Write(JsonConvert.SerializeObject(records));
                using (var writer = new StreamWriter(File.Create(loggerFilePath)))
                {
                    var s = JsonConvert.SerializeObject(taskList);
                    writer.Write(s);
                }
            };
        }

        public void Add(Uri uri, string path, WebHeaderCollection headers)
        {
            lock (taskList)
            {
                taskList.Add(uri.ToString(), path);
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
    }
}


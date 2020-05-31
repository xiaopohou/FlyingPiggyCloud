using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace SixCloudCore.SixTransporter.Downloader
{
    public class HttpDownloader
    {
        public DownloadTaskInfo Info { get; }

        public long Speed { get; set; }

        public DownloadStatusEnum Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    DownloadStatusEnum temp = _status;
                    _status = value;
                    DownloadStatusChangedEvent?.Invoke(temp, value, this);
                }
            }
        }

        public List<DownloadThread> Threads { get; private set; }

        public float DownloadPercentage { get; set; }

        public event Action<DownloadStatusEnum, DownloadStatusEnum, HttpDownloader> DownloadStatusChangedEvent;
        public event EventHandler AllFileStreamDisposed;


        private DownloadStatusEnum _status = DownloadStatusEnum.Waiting;

        public HttpDownloader(DownloadTaskInfo info)
        {
            Info = info;
        }

        public void StartDownload()
        {
            try
            {
                if (Info.ContentSize != 0 && Info.DownloadedSize >= Info.ContentSize)
                {
                    Status = DownloadStatusEnum.Completed;
                    return;
                }
                Status = DownloadStatusEnum.Downloading;
                HttpWebResponse response = GetResponse();
                if (response == null)
                {
                    Status = DownloadStatusEnum.Failed;
                    return;
                }
                if (!File.Exists(Info.DownloadPath))
                {
                    using FileStream _ = File.Create(Info.DownloadPath);
                }

                if (Info.BlockList.Count == 0)
                {
                    Info.ContentSize = response.ContentLength;
                    Info.Init(Info.DownloadPath + ".downloading");
                }
                if (Info.Threads > Info.BlockList.Count(v => !v.Downloaded && !v.Downloading))
                {
                    Info.Threads = Info.BlockList.Count(v => !v.Downloaded && !v.Downloading);
                }

                if (Info.Threads > 8)
                {
                    Info.Threads = 8;
                }
                if (Info.Threads > 8)
                    Info.Threads = 8;
                response.Close();
                Threads?.ToList().ForEach(v => v.ForceStop());
                Threads = new List<DownloadThread>();
                new Thread(ReportDownloadProgress) { IsBackground = true }.Start();
                Info.Limiter.Run();
                for (int i = 0; i < Info.Threads; i++)
                {
                    if (Status == DownloadStatusEnum.Downloading)
                    {
                        DownloadBlock block = Info.BlockList.FirstOrDefault(v => !v.Downloaded && !v.Downloading);
                        if (block == null)
                        {
                            break;
                        }

                        DownloadThread thread = new DownloadThread(block, Info);
                        thread.ThreadCompletedEvent += HttpDownload_ThreadCompletedEvent;
                        thread.ThreadFailedEvent += OnThreadFailedEvent;
                        thread.BeginDownload();
                        Threads.Add(thread);
                    }
                }
            }
            catch
            {
                Status = DownloadStatusEnum.Failed;
            }


        }


        private void OnThreadFailedEvent(DownloadThread _)
        {
            foreach (var thread in Threads)
                thread.ForceStop();
            Status = DownloadStatusEnum.Failed;
        }




        private void HttpDownload_ThreadCompletedEvent(DownloadThread downloadThread)
        {
            lock (this)
            {
                if (Threads.Count == 0)
                {
                    return;
                }

                Threads.Remove(downloadThread);
                if (Info.BlockList.All(v => v.Downloaded))
                {
                    Speed = 0L;
                    DownloadPercentage = 100F;
                    Info.Downloaded = true;
                    Status = DownloadStatusEnum.Completed;
                    Info.Limiter.Stop();
                    return;
                }
                DownloadBlock block = Info.BlockList.FirstOrDefault(v => !v.Downloaded && !v.Downloading);
                if (block == null)
                {
                    return;
                }

                DownloadThread thread = new DownloadThread(block, Info);
                thread.ThreadCompletedEvent += HttpDownload_ThreadCompletedEvent;
                thread.ThreadFailedEvent += OnThreadFailedEvent;
                thread.BeginDownload();
                Threads.Add(thread);
            }
        }


        public DownloadTaskInfo StopAndSave(bool force = false)
        {
            if (Threads != null)
            {
                foreach (DownloadThread thread in Threads)
                {
                    thread.FileStreamDisposed += (sender, e) =>
                    {
                        Threads.Remove(sender as DownloadThread);
                        if (!Threads.Any())
                        {
                            AllFileStreamDisposed?.Invoke(this, EventArgs.Empty);
                        }
                    };
                    if (force)
                    {
                        thread.ForceStop();
                    }
                    else
                    {
                        thread.Stop();
                    }
                }

                Status = DownloadStatusEnum.Paused;
                Info.Limiter.Stop();
                return Info;
            }
            return null;
        }

        private void ReportDownloadProgress()
        {
            long temp = 0L;
            while (Status == DownloadStatusEnum.Downloading)
            {
                Thread.Sleep(1000);
                if (temp == 0)
                {
                    temp = Info.DownloadedSize;
                }
                else
                {
                    if (Status == DownloadStatusEnum.Downloading)
                    {
                        Speed = Info.DownloadedSize - temp;
                        DownloadPercentage = (float)Info.DownloadedSize / Info.ContentSize * 100;
                        temp = Info.DownloadedSize;
                    }
                }
            }
        }
        private HttpWebResponse GetResponse()
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(Info.DownloadUrl) as HttpWebRequest;
                request.Method = "GET";
                //request.UserAgent = "Accelerider-lite download engine";
                request.Timeout = 5000;
                foreach (KeyValuePair<string, string> header in Info.Headers)
                {
                    SetHeaderValue(request.Headers, header.Key, header.Value);
                }

                request.Accept = "*/*";
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
            {
                return null;
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("超时") || ex.Message.Contains("timed out"))
                {
                    return GetResponse();
                }

                Stream stream = ex.Response?.GetResponseStream();
                if (stream == null)
                {
                    //LogHelper.Error("GetResponse failed url:" + sub, false, ex);
                    return null;
                }
                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string result = reader.ReadToEnd();
                    //LogHelper.Info("sub: " + sub + ":" + result);
                }
            }
            catch (OperationCanceledException)
            {
            }

            return null;
        }
        internal static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            System.Reflection.PropertyInfo property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                NameValueCollection collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
    }
    public enum DownloadStatusEnum
    {
        Waiting,
        Downloading,
        Paused,
        Completed,
        Failed
    }
}

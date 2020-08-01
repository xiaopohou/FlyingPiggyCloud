﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace SixCloudCore.SixTransporter.Downloader
{
    public class DownloadThread
    {
        public DownloadBlock Block { get; set; }

        public DownloadTaskInfo Info { get; set; }

        public event Action<DownloadThread> ThreadCompletedEvent;
        internal event EventHandler FileStreamDisposed;
        public event Action<DownloadThread> ThreadFailedEvent;



        private bool _stopped;
        private HttpWebRequest _request;
        private HttpWebResponse _response;

        internal DownloadThread(DownloadBlock block, DownloadTaskInfo info)
        {
            Block = block;
            Info = info;
        }

        public void BeginDownload()
        {
            Block.Downloading = true;
            new Thread(Start) { IsBackground = true }.Start();
        }

        private void Start()
        {
            try
            {
                if (_stopped)
                {
                    return;
                }

                if (!File.Exists(Info.DownloadPath))
                {
                    return;
                }

                if (Block.Downloaded)
                {
                    Block.Downloading = false;
                    ThreadCompletedEvent?.Invoke(this);
                    return;
                }

                if (Block.BeginOffset > Block.EndOffset)
                {
                    Block.Downloading = false;
                    Block.Downloaded = true;
                    ThreadCompletedEvent?.Invoke(this);
                    return;
                }

                _request = WebRequest.CreateHttp(Info.DownloadUrl);
                _request.Method = "GET";
                _request.Timeout = 30000;
                _request.ReadWriteTimeout = 30000;
                foreach (KeyValuePair<string, string> header in Info.Headers)
                {
                    HttpDownloader.SetHeaderValue(_request.Headers, header.Key, header.Value);
                }

                _request.AddRange(Block.BeginOffset, Block.EndOffset);
                _response = (HttpWebResponse)_request.GetResponse();
                using (Stream responseStream = _response.GetResponseStream())
                {
                    try
                    {
                        using (FileStream stream = new FileStream(Info.DownloadPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 1024 * 1024))
                        {
                            stream.Seek(Block.BeginOffset, SeekOrigin.Begin);
                            byte[] array = new byte[1024];
                            int i = responseStream.Read(array, 0, array.Length);
                            while (true)
                            {
                                if (_stopped)
                                {
                                    stream.Flush();
                                    Block.Downloading = false;
                                    return;
                                }

                                if (i <= 0 && Block.BeginOffset - 1 != Block.EndOffset)
                                {
                                    new Thread(Start) { IsBackground = true }.Start();
                                    return;
                                }

                                if (i <= 0 || Block.BeginOffset > Block.EndOffset)
                                {
                                    break;
                                }

                                stream.Write(array, 0, i);
                                Block.BeginOffset += i;
                                Block.DownloadedSize += i;
                                Info.DownloadedSize += i;
                                Info.Limiter.Downloaded(i);
                                i = responseStream.Read(array, 0, array.Length);
                            }

                            stream.Flush();
                        }

                    }
                    finally
                    {
                        if (_stopped)
                        {
                            FileStreamDisposed?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }

                Block.Downloaded = true;
                Block.Downloading = false;
                ThreadCompletedEvent?.Invoke(this);
            }
            catch (WebException)
            {
                if (Block.BeginOffset == Block.EndOffset)
                {
                    Block.Downloaded = true;
                    Block.Downloading = false;
                    ThreadCompletedEvent?.Invoke(this);
                    return;
                }
                if (Info.RetryCount < Info.MaxRetry)
                {
                    Thread.Sleep(1000);
                    new Thread(Start) { IsBackground = true }.Start();
                    return;
                }
                ThreadFailedEvent?.Invoke(this);
            }
            catch (Exception)
            {
                if (Info.RetryCount < Info.MaxRetry)
                {
                    Thread.Sleep(1000);
                    new Thread(Start) { IsBackground = true }.Start();
                    return;
                }
                ThreadFailedEvent?.Invoke(this);
            }
            finally
            {
                try
                {
                    _response?.Close();
                    _request?.Abort();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (Block.Downloaded)
            {
                return;
            }

            _stopped = true;
        }
        public void ForceStop()
        {
            if (Block.Downloaded)
            {
                return;
            }

            _stopped = true;
            try
            {
                _response?.Close();
                _request?.Abort();
            }
            catch
            {
            }
            Block.Downloading = false;
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace FileDownloader
{
    public delegate Uri RefreshUri();

    public class FileDownloadTask : IFileDownloader, ISplittableTask
    {
        /// <summary>
        /// 文件名（来自构造函数）
        /// </summary>
        private readonly string fileName;
        /// <summary>
        /// 下载任务保存的本地路径
        /// </summary>
        private readonly string localPath;
        /// <summary>
        /// 用于获取下载链接的函数指针
        /// </summary>
        private readonly RefreshUri flushUri;
        private IEnumerable<int> AchieveDataStream(byte[] binaryBuffer)
        {
            if (File.Exists(LocalFileName + ".ezdlpart") && new FileInfo(LocalFileName + ".ezdlpart").Length != BytesReceived)
            {
                File.Delete(LocalFileName + ".ezdlpart");
            }

            Uri uri = flushUri();
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            request.AddRange(BytesReceived);
            HttpWebResponse result = request.GetResponse() as HttpWebResponse;
            TotalBytesToReceive = int.Parse(Regex.Split(result.Headers[HttpResponseHeader.ContentRange], "/")[1]);
            return CreateBlockEnumerator(binaryBuffer, result.GetResponseStream());

            IEnumerable<int> CreateBlockEnumerator(byte[] bytes, Stream stream)
            {
                int readCount;
                while (true)
                {
                    readCount = stream.Read(bytes, 0, bytes.Length);
                    if (readCount <= 0)
                    {
                        stream.Close();
                        yield break;
                    }
                    else
                    {
                        BytesReceived += readCount;
                        DownloadProgressChanged?.Invoke(this, new DownloadFileProgressChangedArgs((int)(TotalBytesToReceive == 0 ? 0 : BytesReceived / TotalBytesToReceive), BytesReceived, TotalBytesToReceive));
                        yield return readCount;
                    }
                }
            }
        }
        void ISplittableTask.AchieveSlice(byte[] binaryBuffer)
        {
            foreach (var sliceSize in AchieveDataStream(binaryBuffer))
            {

                using FileStream targetFile = new FileStream(LocalFileName + ".ezdlpart", FileMode.Append, FileAccess.Write);
                targetFile.Write(binaryBuffer, 0, sliceSize);
                targetFile.Flush();
                if (!IsRunning)
                {
                    return;
                }
            }
            File.Move(LocalFileName + ".ezdlpart", LocalFileName);
            DownloadFileCompleted?.Invoke(this, new DownloadFileCompletedArgs(CompletedState.Succeeded, fileName, null, TimeSpan.Zero, TotalBytesToReceive, BytesReceived, null));
        }

        public string LocalFileName => Path.Combine(localPath, fileName);
        public bool IsRunning { get; private set; } = false;
        public long BytesReceived { get; private set; }
        public long TotalBytesToReceive { get; private set; }
        DownloadPorter ISplittableTask.CurrentWorker { get; set; }

        //bool ISplittableTask.IsRunning => throw new NotImplementedException();

        public event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;
        public event EventHandler<DownloadFileProgressChangedArgs> DownloadProgressChanged;


        /// <summary>
        /// 取消下载
        /// </summary>
        public void Cancel()
        {
            IsRunning = false;
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void Pause()
        {
            IsRunning = false;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {
            DownloadFactory.Add(this);
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
        }

        public FileDownloadTask(string destinationDirectory, RefreshUri getDownloadUri, string name, long bytesReceived = 0)
        {
            localPath = destinationDirectory;
            Directory.CreateDirectory(localPath);
            flushUri = getDownloadUri;
            fileName = name;
            BytesReceived = bytesReceived;

        }
    }
}

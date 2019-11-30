using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace FileDownloader
{
    public delegate Uri DownloadUriInvalideEventHandler();

    public class FileDownloadTask : IFileDownloader
    {
        //缓冲区128kb
        private const int bufferSize = 1024 * 128;
        private const int speedLimit = 0;

        private bool isRunning = false;
        private IEnumerator<int> downloadEnumerator;
        private readonly string fileName;
        private readonly byte[] binaryBuffer = new byte[bufferSize];
        private readonly string localPath;
        private readonly DownloadUriInvalideEventHandler flushUri;

        internal IEnumerable<int> CreateBlockEnumerator(byte[] bytes, Stream stream)
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

        internal bool MoveNext()
        {
            if (downloadEnumerator.MoveNext())
            {
                using (FileStream targetFile = new FileStream(LocalFileName + ".ezdlpart", FileMode.Append, FileAccess.Write))
                {
                    targetFile.Write(binaryBuffer, 0, downloadEnumerator.Current);
                    targetFile.Flush();
                }
                return true;
            }
            else
            {
                File.Move(LocalFileName + ".ezdlpart", LocalFileName);
                DownloadFileCompleted?.Invoke(this, new DownloadFileCompletedArgs(CompletedState.Succeeded, fileName, null, TimeSpan.Zero, TotalBytesToReceive, BytesReceived, null));
                return false;
            }
        }

        public long BytesReceived { get; private set; }

        public long TotalBytesToReceive { get; private set; }

        //public CompletedState CompletedState { get; protected set; }

        public event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;
        public event EventHandler<DownloadFileProgressChangedArgs> DownloadProgressChanged;

        /// <summary>
        /// 取消下载
        /// </summary>
        public void Cancel()
        {
            isRunning = false;
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void Pause()
        {
            isRunning = false;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;

            //暂不支持重启后的断点续传
            if (File.Exists(LocalFileName + ".ezdlpart") && new FileInfo(LocalFileName + ".ezdlpart").Length != BytesReceived)
            {
                File.Delete(LocalFileName + ".ezdlpart");
            }

            Uri uri = flushUri();
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            request.AddRange(BytesReceived);
            HttpWebResponse result;
            try
            {
                result = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException)
            {
                return;
            }

            string totalBytes = Regex.Split(result.Headers[HttpResponseHeader.ContentRange], "/")[1];
            TotalBytesToReceive = int.Parse(totalBytes);
            downloadEnumerator = CreateBlockEnumerator(binaryBuffer, result.GetResponseStream()).GetEnumerator();
            while (MoveNext() && isRunning)
            {

            }
        }

        public FileDownloadTask(string destinationDirectory, DownloadUriInvalideEventHandler getDownloadUri, string name, long bytesReceived = 0)
        {
            localPath = destinationDirectory;
            Directory.CreateDirectory(localPath);
            flushUri = getDownloadUri;
            fileName = name;
            BytesReceived = bytesReceived;
        }

        /// <summary>
        /// 获取下载文件的本地路径含文件名
        /// </summary>
        public string LocalFileName => Path.Combine(localPath, fileName);
    }
}

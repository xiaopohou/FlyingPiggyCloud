using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace FileDownloader
{
    /// <summary>
    /// FileDownloader interface
    /// </summary>
    public interface IFileDownloader
    {
        /// <summary>
        /// Fired when download is finished, even if it's failed.
        /// </summary>
        event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;

        /// <summary>
        /// Fired when download progress is changed.
        /// </summary>
        event EventHandler<DownloadFileProgressChangedArgs> DownloadProgressChanged;

        /// <summary>
        /// Gets the total bytes received so far
        /// </summary>
        long BytesReceived { get; }

        /// <summary>
        /// Gets the total bytes to receive
        /// </summary>
        long TotalBytesToReceive { get; }

        CompletedState CompletedState { get; }

        /// <summary>
        /// Cancel current download
        /// </summary>
        void Cancel();

        void Start();

        string LocalFileName { get; }

        void Pause();
    }

    public class FileDownloadTask : IFileDownloader
    {
        //缓冲区128kb
        private const int bufferSize = 1024 * 128;
        private const int speedLimit = 0;

        private string fileName;

        //private readonly Uri sourceUri;
        private readonly byte[] binaryBuffer = new byte[bufferSize];
        private IEnumerator<int> downloadEnumerator;
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
                    yield return readCount;
                }
            }
        }

        internal bool MoveNext()
        {
            if (downloadEnumerator.MoveNext())
            {
                using (var targetFile = new FileStream(LocalFileName + ".ezdlpart", FileMode.Append))
                {
                    targetFile.Write(binaryBuffer, 0, downloadEnumerator.Current);
                    targetFile.Flush();
                }
                return true;
            }
            else
            {
                File.Move(LocalFileName + ".ezdlpart", LocalFileName);
                return false;
            }
        }

        public long BytesReceived { get; private set; } = 0;

        public long TotalBytesToReceive { get; private set; }

        public CompletedState CompletedState { get; protected set; }

        public event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;
        public event EventHandler<DownloadFileProgressChangedArgs> DownloadProgressChanged;


        public void Cancel()
        {
            throw new NotImplementedException();
        }
        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
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

            var totalBytes = Regex.Split(result.Headers[HttpResponseHeader.ContentRange], "/")[1];
            TotalBytesToReceive = int.Parse(totalBytes);
            downloadEnumerator = CreateBlockEnumerator(binaryBuffer, result.GetResponseStream()).GetEnumerator();
            //fileName = Guid.NewGuid().ToString();
            //fileName = fileName ?? new ContentDisposition(result.Headers["Content-Disposition"]).FileName;
            //while (MoveNext())
            //{

            //}
        }

        public FileDownloadTask(string destinationDirectory, DownloadUriInvalideEventHandler getDownloadUri)
        {
            localPath = destinationDirectory;
            flushUri = getDownloadUri;
        }

        /// <summary>
        /// 获取下载文件的本地路径含文件名
        /// </summary>
        public string LocalFileName => Path.Combine(localPath, fileName);
    }

    public delegate Uri DownloadUriInvalideEventHandler();
}

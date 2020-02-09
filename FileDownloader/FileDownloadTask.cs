using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader
{
    public delegate Uri RefreshUri();


    [Serializable]
    public class DownloadException : Exception
    {
        public DownloadException() { }
        public DownloadException(string message) : base(message) { }
        public DownloadException(string message, Exception inner) : base(message, inner) { }
        protected DownloadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

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
        private async Task<IEnumerable<int>> AchieveDataStream(HttpClient httpClient, byte[] binaryBuffer)
        {
            if (File.Exists(LocalFileName + ".ezdlpart") && new FileInfo(LocalFileName + ".ezdlpart").Length != BytesReceived)
            {
                File.Delete(LocalFileName + ".ezdlpart");
            }

            Uri uri = flushUri();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Range = new RangeHeaderValue(BytesReceived, null);
            var result = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            TotalBytesToReceive = result.Content.Headers.ContentRange.Length ?? 0;
            return CreateBlockEnumerator(binaryBuffer, await result.Content.ReadAsStreamAsync());

            static IEnumerable<int> CreateBlockEnumerator(byte[] bytes, Stream stream)
            {
                int readCount;
                while (true)
                {
                    try
                    {
                        readCount = stream.Read(bytes, 0, bytes.Length);
                    }
                    catch (IOException ex)
                    {
                        throw new DownloadException(ex.Message, ex);
                    }
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
        }
        async Task ISplittableTask.AchieveSlice(HttpClient httpClient, byte[] binaryBuffer)
        {
            foreach (var sliceSize in await AchieveDataStream(httpClient, binaryBuffer))
            {
                bool success = true;
                do
                {
                    try
                    {
                        using FileStream targetFile = new FileStream(LocalFileName + ".ezdlpart", FileMode.Append, FileAccess.Write);
                        targetFile.Write(binaryBuffer, 0, sliceSize);
                        targetFile.Flush();
                    }
                    catch (InvalidOperationException)
                    {
                        success = false;
                    }
                    catch (IOException)
                    {
                        success = false;
                    }
                } while (success == false);
                BytesReceived += sliceSize;
                DownloadProgressChanged?.Invoke(this, new DownloadFileProgressChangedArgs((int)(TotalBytesToReceive == 0 ? 0 : BytesReceived / TotalBytesToReceive), BytesReceived, TotalBytesToReceive));
                if (!IsRunning)
                {
                    return;
                }
            }

            bool moveSuccess = true;
            do
            {
                try
                {
                    string targetFileName;
                    var index = 0;
                    do
                    {
                        targetFileName = Path.Combine(localPath, Path.GetFileNameWithoutExtension(fileName) + (index == 0 ? "" : $"（{index}）") + Path.GetExtension(fileName));
                        index++;
                    } while (File.Exists(targetFileName));
                    File.Move(LocalFileName + ".ezdlpart", targetFileName);
                }
                catch (DirectoryNotFoundException)
                {
#warning 某些特定的文件名会导致抛出此异常
                    moveSuccess = false;
                }

            } while (moveSuccess == false);
            DownloadFileCompleted?.Invoke(this, new DownloadFileCompletedArgs(CompletedState.Succeeded, fileName, null, TimeSpan.Zero, TotalBytesToReceive, BytesReceived, null));
        }

        public string LocalFileName => Path.Combine(localPath, fileName);
        public bool IsRunning { get; private set; } = false;
        public long BytesReceived { get; private set; }
        public long TotalBytesToReceive { get; private set; }

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

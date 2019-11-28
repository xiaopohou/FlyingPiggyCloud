using System;

namespace FileDownloader
{
    /// <summary>
    /// FileDownloader interface
    /// </summary>
    public interface IFileDownloader : IDisposable
    {
        /// <summary>
        /// Fired when download is finished, even if it's failed.
        /// </summary>
        event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;

        /// <summary>
        /// Fired when download progress is changed.
        /// </summary>
        event EventHandler<DownloadFileProgressChangedArgs> DownloadProgressChanged;

        ///// <summary>
        ///// Gets or sets the delay between download attempts. 
        ///// </summary>
        //TimeSpan DelayBetweenAttempts { get; set; }

        ///// <summary>
        ///// Gets or sets the maximum waiting timeout for pending request to be finished. Default is 15 seconds.
        ///// </summary>
        //TimeSpan SafeWaitTimeout { get; set; }

        ///// <summary>
        ///// Gets or sets the maximum number of download attempt.
        ///// </summary>
        //int MaxAttempts { get; set; }

        /// <summary>
        /// Gets the total bytes received so far
        /// </summary>
        long BytesReceived { get; }

        /// <summary>
        /// Gets the total bytes to receive
        /// </summary>
        long TotalBytesToReceive { get; }

        /// <summary>
        /// Start async download of source to destinationPath. destinationPath should be full path with file name.
        /// </summary>
        /// <param name="source">Source URI</param>
        /// <param name="destinationPath">Destination path</param>
        void DownloadFile(Uri source, string destinationPath);

        /// <summary>
        /// Start download of source file to downloadDirectory. File would be saved with filename taken from server 
        /// </summary>
        /// <param name="source">Source URI</param>
        /// <param name="destinationDirectory">Destination directory</param>
        void DownloadFilePreserveServerFileName(Uri source, string destinationDirectory);

        /// <summary>
        /// Cancel current download
        /// </summary>
        void CancelDownloadAsync();

        string GetLocalFileName();

        void Pause();
    }

    public class FileDownloadTask : IFileDownloader
    {
        public long BytesReceived => throw new NotImplementedException();

        public long TotalBytesToReceive => throw new NotImplementedException();

        public event EventHandler<DownloadFileCompletedArgs> DownloadFileCompleted;
        public event EventHandler<DownloadFileProgressChangedArgs> DownloadProgressChanged;

        public void CancelDownloadAsync()
        {
            throw new NotImplementedException();
        }

        public void DownloadFile(Uri source, string destinationPath)
        {
            throw new NotImplementedException();
        }

        public void DownloadFilePreserveServerFileName(Uri source, string destinationDirectory)
        {
            throw new NotImplementedException();
        }

        public string GetLocalFileName()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~FileDownloadTask()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}

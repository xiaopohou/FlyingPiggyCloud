using System;

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

        /// <summary>
        /// Cancel current download
        /// </summary>
        void Cancel();

        void Start();

        string LocalFileName { get; }

        void Pause();
    }
}

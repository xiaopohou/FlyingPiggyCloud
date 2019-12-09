using System;
using System.Collections.Generic;
using System.Text;

namespace FileDownloader
{
    internal class DownloadPotter
    {
        private const int bufferSize = 1024 * 512;
        private const int speedLimit = 0;

        private readonly byte[] binaryBuffer = new byte[bufferSize];

        internal void NextJob(ISplittableTask task)
        {
            task.AchieveDataStream(binaryBuffer);
            while (task.IsRunning && task.MoveNext(binaryBuffer))
            {

            }
        }

    }

    internal interface ISplittableTask
    {
        internal void AchieveDataStream(byte[] binaryBuffer);

        internal bool MoveNext(byte[] binaryBuffer);

        public bool IsRunning { get; }
    }

    internal static class DownloadFactory
    {
        private static readonly List<FileDownloadTask> tasks = new List<FileDownloadTask>(8);

        private static readonly DownloadPotter[] potters = new DownloadPotter[5];

        static DownloadFactory()
        {

        }
    }
}

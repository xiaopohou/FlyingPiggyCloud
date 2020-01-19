using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FileDownloader
{
    internal class DownloadPorter
    {
        private const int bufferSize = 1024 * 512;
        private const int speedLimit = 0;
        private readonly byte[] binaryBuffer = new byte[bufferSize];
        private readonly HttpClient httpClient = new HttpClient();


        internal async Task NextJob(ISplittableTask task)
        {
            try
            {
                bool error;
                do
                {
                    try
                    {
                        error = false;
                        await task.AchieveSlice(httpClient, binaryBuffer);
                    }
                    catch (DownloadException)
                    {
                        error = true;
                        Thread.Sleep(500);
                    }

                } while (error);

            }
            finally
            {
                DownloadFactory.Add(this);
            }
        }

    }
}

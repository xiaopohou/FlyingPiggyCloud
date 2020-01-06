using System.Net.Http;
using System.Threading.Tasks;

namespace FileDownloader
{
    internal interface ISplittableTask
    {

        internal Task AchieveSlice(HttpClient httpClient, byte[] binaryBuffer);

        public bool IsRunning { get; }
    }
}

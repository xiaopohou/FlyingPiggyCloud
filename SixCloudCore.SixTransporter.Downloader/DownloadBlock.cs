using Newtonsoft.Json;

namespace SixCloudCore.SixTransporter.Downloader
{
    public class DownloadBlock
    {
        public long BeginOffset { get; set; }

        public long EndOffset { get; set; }

        public long DownloadedSize { get; set; }

        [JsonIgnore]
        public bool Downloading { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Downloaded { get; set; }
    }
}

using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    /// <summary>
    /// 指示一个离线任务url解析结果中的一个文件
    /// </summary>
    public class OfflineTaskParseFile : EntityBodyBase
    {
        [JsonIgnore]
        public bool? IsChecked { get; set; } = true;

        [JsonProperty(PropertyName = "downloadIdentity")]
        public string DownloadIdentity { get; set; }

        [JsonProperty(PropertyName = "pathIdentity")]
        public string PathIdentity { get; set; }

        [JsonProperty(PropertyName = "createTime")]
        public long CreateTime { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "downloadSize")]
        public long DownloadSize { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "flag")]
        public int Flag { get; set; }

        [JsonProperty(PropertyName = "fileIndex")]
        public int FileIndex { get; set; }

        [JsonProperty(PropertyName = "finish")]
        public bool Finish { get; set; }

    }
}

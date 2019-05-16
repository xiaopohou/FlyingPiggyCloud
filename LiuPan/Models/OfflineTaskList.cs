using Newtonsoft.Json;

namespace SixCloud.Models
{
    internal class OfflineTaskList
    {
        public class OfflineTask
        {
            [JsonProperty(PropertyName = "identity")]
            public string Identity { get; set; }

            [JsonProperty(PropertyName = "userIdentity")]
            public int UserIdentity { get; set; }

            [JsonProperty(PropertyName = "createTime")]
            public long CreateTime { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "type")]
            public int Type { get; set; }

            [JsonProperty(PropertyName = "status")]
            public int Status { get; set; }

            [JsonProperty(PropertyName = "size")]
            public long Size { get; set; }

            [JsonProperty(PropertyName = "downloadSize")]
            public long DownloadSize { get; set; }

            [JsonProperty(PropertyName = "progress")]
            public double Progress { get; set; }

            [JsonProperty(PropertyName = "cip")]
            public string Cip { get; set; }

            [JsonProperty(PropertyName = "data")]
            public string Data { get; set; }
        }

        [JsonProperty(PropertyName ="list")]
        public OfflineTask[] List { get; set; }
    }
}

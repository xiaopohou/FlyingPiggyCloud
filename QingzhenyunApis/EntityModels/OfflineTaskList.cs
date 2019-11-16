using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class OfflineTaskList
    {

        [JsonProperty(PropertyName = "list")]
        public OfflineTask[] List { get; set; }

        [JsonProperty(PropertyName = "totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty(PropertyName = "totalPage")]
        public int TotalPage { get; set; }

        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }

        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }

    }

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
}

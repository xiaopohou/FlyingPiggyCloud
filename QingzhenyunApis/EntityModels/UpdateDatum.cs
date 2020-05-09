using Newtonsoft.Json;
using System;

namespace QingzhenyunApis.EntityModels
{
    public partial class UpdateDatum
    {
        [JsonProperty("identity")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Identity { get; set; }

        [JsonProperty("numberVersion")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long NumberVersion { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("force")]
        public bool Force { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("debug")]
        public bool Debug { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("downloadAddress")]
        public Uri DownloadAddress { get; set; }

        [JsonProperty("storeAddress")]
        public string StoreAddress { get; set; }

        [JsonProperty("websiteAddress")]
        public Uri WebsiteAddress { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("createTime")]
        public string CreateTime { get; set; }

        [JsonProperty("size")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Size { get; set; }
    }
}

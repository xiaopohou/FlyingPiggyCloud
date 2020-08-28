using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class SocketMessage : EntityBodyBase
    {
        [JsonProperty("identity")]
        public string Identity { get; set; }

        [JsonProperty("userIdentity")]
        public long UserIdentity { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("progress")]
        public long Progress { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("source")]
        public FileMetaData Source { get; set; }

        [JsonProperty("destination")]
        public FileMetaData Destination { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }
}

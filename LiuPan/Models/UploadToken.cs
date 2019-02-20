using Newtonsoft.Json;

namespace SixCloud.Models
{
    public class UploadToken
    {
        [JsonProperty(PropertyName = "userId")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "uploadUrl")]
        public string UploadUrl { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }
    }
}

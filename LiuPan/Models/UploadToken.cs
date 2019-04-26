using Newtonsoft.Json;

namespace SixCloud.Models
{
    public class UploadToken
    {
        [JsonProperty(PropertyName = "hashCached")]
        public bool HashCached { get; set; }


        [JsonProperty(PropertyName = "uploadInfo")]
        public Information UploadInfo { get; set; }

        public class Information
        {
            [JsonProperty(PropertyName = "uploadToken")]
            public string Token { get; set; }

            [JsonProperty(PropertyName = "type")]
            public int Type { get; set; }

            [JsonProperty(PropertyName = "uploadUrl")]
            public string UploadUrl { get; set; }

            [JsonProperty(PropertyName = "filePath")]
            public string FilePath { get; set; }
        }
    }
}

using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class UploadToken
    {
        [JsonProperty(PropertyName = "uploadInfo")]
        public Information UploadInfo { get; set; }

        [JsonProperty(PropertyName = "hashCached")]
        public bool HashCached { get; set; }

        public class Information
        {
            [JsonProperty(PropertyName = "uploadToken")]
            public string Token { get; set; }

            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "uploadUrl")]
            public string UploadUrl { get; set; }

            [JsonProperty(PropertyName = "filePath")]
            public string FilePath { get; set; }
        }
    }
}

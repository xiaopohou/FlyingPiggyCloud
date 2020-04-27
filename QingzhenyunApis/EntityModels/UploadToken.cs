using Newtonsoft.Json;
using System;

namespace QingzhenyunApis.EntityModels
{
    public class UploadToken : EntityBodyBase
    {
        [JsonProperty("uploadToken", Required = Required.Always)]
        public string UploadTokenUploadToken { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("created")]
        public bool Created { get; set; }

        [JsonProperty("partUploadUrl")]
        public Uri PartUploadUrl { get; set; }

        [JsonProperty("directUploadUrl")]
        public Uri DirectUploadUrl { get; set; }
    }
}

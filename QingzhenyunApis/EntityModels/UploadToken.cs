using Newtonsoft.Json;
using System;

namespace QingzhenyunApis.EntityModels
{
    public class UploadToken
    {
        [JsonProperty("uploadToken",Required = Required.Always)]
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



        //[JsonProperty(PropertyName = "uploadInfo")]
        //public Information UploadInfo { get; set; }

        //[JsonProperty(PropertyName = "hashCached")]
        //public bool HashCached { get; set; }

        //public class Information
        //{
        //    [JsonProperty(PropertyName = "uploadToken")]
        //    public string Token { get; set; }

        //    [JsonProperty(PropertyName = "type")]
        //    public string Type { get; set; }

        //    [JsonProperty(PropertyName = "uploadUrl")]
        //    public string UploadUrl { get; set; }

        //    [JsonProperty(PropertyName = "filePath")]
        //    public string FilePath { get; set; }
        //}
    }
}

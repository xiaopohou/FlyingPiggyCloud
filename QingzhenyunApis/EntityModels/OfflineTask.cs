using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class OfflineTask : EntityBodyBase
    {
        [JsonProperty("taskIdentity")]
        public string TaskIdentity { get; set; }

        [JsonProperty("userIdentity")]
        public long UserIdentity { get; set; }

        [JsonProperty("createTime")]
        public long CreateTime { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("processedSize")]
        public long ProcessedSize { get; set; }

        [JsonProperty("progress")]
        public long Progress { get; set; }

        [JsonProperty("errorCode")]
        public long ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("savePath")]
        public string SavePath { get; set; }

        [JsonProperty("saveIdentity")]
        public string SaveIdentity { get; set; }

        [JsonProperty("accessPath")]
        public string AccessPath { get; set; }

        [JsonProperty("accessIdentity")]
        public string AccessIdentity { get; set; }

        [JsonProperty("fileMime")]
        public string FileMime { get; set; }

        [JsonProperty("fileType")]
        public long FileType { get; set; }

        [JsonProperty("createAddress")]
        public string CreateAddress { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("textLink")]
        public string TextLink { get; set; }

        [JsonProperty("fileHash")]
        public string FileHash { get; set; }

        [JsonProperty("op")]
        public long Op { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("kind")]
        public long Kind { get; set; }
    }
}

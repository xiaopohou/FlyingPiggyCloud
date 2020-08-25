using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    /// <summary>
    /// 操作文件系统的返回体
    /// </summary>
    public class FileSystemOperate : EntityBodyBase
    {
        [JsonProperty(PropertyName = "identity")]
        public string Identity { get; set; }

        [JsonProperty(PropertyName = "data")]
        public int Data { get; set; }

        [JsonProperty(PropertyName = "async")]
        public bool Async { get; set; }
    }

}

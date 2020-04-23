using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace QingzhenyunApis.EntityModels
{
    /// <summary>
    /// 一个离线下载链接的解析结果
    /// </summary>
    public class OfflineTaskParseUrl
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("info")]
        public OfflineTaskParseInfo Info { get; set; }
    }
}

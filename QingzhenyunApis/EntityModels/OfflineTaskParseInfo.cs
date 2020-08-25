using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class OfflineTaskParseInfo : DataListResult<OfflineTaskParseFile>
    {
        [JsonProperty("textLink")]
        public string TextLink { get; set; }

        [JsonProperty("fileHash")]
        public string FileHash { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("identity")]
        public string Identity { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        //[JsonProperty("dataList")]
        //public IList<OfflineTaskParseFile> DataList { get; set; }
    }
}

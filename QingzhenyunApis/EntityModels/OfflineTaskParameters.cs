using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class OfflineTaskParameters : EntityBodyBase
    {
        public OfflineTaskParameters(string hash, string[] ignoreFiles = null)
        {
            Hash = hash;
            Ignores = ignoreFiles;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "hash")]
        public string Hash { get; set; }

        /// <summary>
        /// 所在文件的 PathIdentity
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ignore")]
        public IList<string> Ignores { get; set; }
    }
}

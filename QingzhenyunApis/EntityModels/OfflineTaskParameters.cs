using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class OfflineTaskParameters
    {
        public OfflineTaskParameters(string identity, string[] iginreFiles=null)
        {
            Identity = identity;
            IginreFiles = iginreFiles;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "identity")]
        public string Identity { get; set; }

        /// <summary>
        /// 所在文件的 PathIdentity
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "iginreFiles")]
        public IList<string> IginreFiles { get; set; }
    }
}

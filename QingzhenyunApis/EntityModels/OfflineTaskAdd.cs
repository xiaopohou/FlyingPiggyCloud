using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class OfflineTaskAdd : EntityBodyBase
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
}

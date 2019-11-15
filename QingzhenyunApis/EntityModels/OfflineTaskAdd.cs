using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    internal class OfflineTaskAdd
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
}

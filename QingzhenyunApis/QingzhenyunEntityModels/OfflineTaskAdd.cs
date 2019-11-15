using Newtonsoft.Json;

namespace QingzhenyunApis.QingzhenyunEntityModels
{
    internal class OfflineTaskAdd
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
}

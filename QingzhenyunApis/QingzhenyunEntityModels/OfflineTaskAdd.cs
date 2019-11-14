using Newtonsoft.Json;

namespace SixCloud.Models
{
    internal class OfflineTaskAdd
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
}

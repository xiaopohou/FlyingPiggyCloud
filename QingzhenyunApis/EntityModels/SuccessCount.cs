using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class SuccessCount
    {
        [JsonProperty(PropertyName = "successCount")]
        public int Value { get; set; }
    }

}

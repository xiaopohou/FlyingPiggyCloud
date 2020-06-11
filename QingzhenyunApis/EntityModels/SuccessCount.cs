using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class SuccessCount : EntityBodyBase
    {
        [JsonProperty(PropertyName = "successCount")]
        public int Value { get; set; }
    }
}

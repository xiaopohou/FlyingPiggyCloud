using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class SubscriptionPlan : EntityBodyBase
    {
        [JsonProperty("identity")]
        public long Identity { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("periodType")]
        public long PeriodType { get; set; }

        [JsonProperty("supportedPayType")]
        public string SupportedPayType { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}

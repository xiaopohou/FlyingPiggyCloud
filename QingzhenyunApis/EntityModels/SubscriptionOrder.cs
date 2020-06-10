using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class SubscriptionOrder : EntityBodyBase
    {
        [JsonProperty("identity")]
        public string Identity { get; set; }

        [JsonProperty("userIdentity")]
        public long UserIdentity { get; set; }

        [JsonProperty("planIdentity")]
        public long PlanIdentity { get; set; }

        [JsonProperty("planType")]
        public long PlanType { get; set; }

        [JsonProperty("payType")]
        public long PayType { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("createTime")]
        public long CreateTime { get; set; }

        [JsonProperty("refreshTime")]
        public long RefreshTime { get; set; }

        [JsonProperty("ext")]
        public string Ext { get; set; }

        [JsonProperty("expireTime")]
        public long ExpireTime { get; set; }

        [JsonProperty("createAddress")]
        public string CreateAddress { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("completeTime")]
        public long CompleteTime { get; set; }

        [JsonProperty("planDescription")]
        public string PlanDescription { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }
    }
}

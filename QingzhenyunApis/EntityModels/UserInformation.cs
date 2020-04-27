using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class UserInformation : EntityBodyBase
    {
        [JsonProperty(PropertyName = "identity")]
        public int UUID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "createTime")]
        public long CreateTime { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "spaceUsed")]
        public long SpaceUsed { get; set; }

        [JsonProperty(PropertyName = "spaceCapacity")]
        public long SpaceCapacity { get; set; }

        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "vip")]
        public int Vip { get; set; }

        [JsonProperty(PropertyName = "vipExpireTime")]
        public long VipExpireTime { get; set; }

        [JsonProperty(PropertyName = "lastActivateTime")]
        public long LastActivateTime { get; set; }
    }
}

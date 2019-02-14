using Newtonsoft.Json;

namespace LiuPan.Models
{
    public class UserInformation
    {
        [JsonProperty(PropertyName = "uuid")]
        public int UUID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        [JsonProperty(PropertyName = "createTime")]
        public long CreateTime { get; set; }

        [JsonProperty(PropertyName = "createIp")]
        public string CreateIp { get; set; }

        //[JsonProperty(PropertyName = "ssid")]
        //public object SSID { get; set; }

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
    }
}

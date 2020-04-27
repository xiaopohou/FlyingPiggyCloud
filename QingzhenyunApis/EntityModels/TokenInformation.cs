using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class TokenInformation : EntityBodyBase
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

    }
}

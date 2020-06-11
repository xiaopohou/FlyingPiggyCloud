using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class TicketInformation : EntityBodyBase
    {
        [JsonProperty("identity")]
        public long Identity { get; set; }

        [JsonProperty("userIdentity")]
        public long UserIdentity { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("replyUserIdentity")]
        public long ReplyUserIdentity { get; set; }

        [JsonProperty("type")]
        public TicketType Type { get; set; }

        [JsonProperty("status")]
        public TicketStatus Status { get; set; }

        [JsonProperty("createTime")]
        public long CreateTime { get; set; }

        [JsonProperty("refreshTime")]
        public long RefreshTime { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("images")]
        public string Images { get; set; }
    }
}

using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class TicketReplyList : DataListResult<TicketInformation>
    {
        [JsonProperty("ticket")]
        public TicketInformation Ticket { get; set; }
    }
}

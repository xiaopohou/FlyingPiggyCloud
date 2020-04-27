using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class DailyQuota
    {
        [JsonProperty("dailyQuota")]
        public long DailyQuotaDailyQuota { get; set; }

        [JsonProperty("dailyUsed")]
        public long DailyUsed { get; set; }
    }

}

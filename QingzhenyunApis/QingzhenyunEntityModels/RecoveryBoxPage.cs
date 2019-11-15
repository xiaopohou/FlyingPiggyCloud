using Newtonsoft.Json;

namespace QingzhenyunApis.QingzhenyunEntityModels
{
    internal class RecoveryBoxPage
    {
        [JsonProperty(PropertyName = "list")]
        public RecoveryBoxItem[] List { get; set; }

        [JsonProperty(PropertyName = "totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty(PropertyName = "totalPage")]
        public int TotalPage { get; set; }

        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }

        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }
    }
}

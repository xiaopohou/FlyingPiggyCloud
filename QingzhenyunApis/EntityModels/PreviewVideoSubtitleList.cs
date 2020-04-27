using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class PreviewVideoSubtitleList : EntityBodyBase
    {
        [JsonProperty(PropertyName = "dataList")]
        public List<Subtitle> DataList { get; set; }
    }
}

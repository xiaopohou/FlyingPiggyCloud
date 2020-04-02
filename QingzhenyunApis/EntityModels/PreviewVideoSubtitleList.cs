using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class PreviewVideoSubtitleList
    {
        [JsonProperty(PropertyName = "dataList")]
        public List<Subtitle> DataList { get; set; }
    }
}

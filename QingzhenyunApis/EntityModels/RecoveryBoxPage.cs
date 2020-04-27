using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class RecoveryBoxPage : EntityBodyBase
    {
        [JsonProperty(PropertyName = "dataList")]
        public List<RecoveryBoxItem> List { get; set; }
    }
}

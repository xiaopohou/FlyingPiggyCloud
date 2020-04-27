using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class OfflineTaskList : EntityBodyBase
    {
        [JsonProperty(PropertyName = "dataList")]
        public List<OfflineTask> List { get; set; }
    }
}

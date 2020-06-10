using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class DataListResult<T> : EntityBodyBase
    {
        [JsonProperty("dataList")]
        public IList<T> DataList { get; set; }
    }
}

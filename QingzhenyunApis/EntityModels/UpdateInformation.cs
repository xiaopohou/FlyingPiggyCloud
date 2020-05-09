using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public partial class UpdateInformation
    {
        [JsonProperty("data")]
        public List<UpdateDatum> Data { get; set; }
    }
}

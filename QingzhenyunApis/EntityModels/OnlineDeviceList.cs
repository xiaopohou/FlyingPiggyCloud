using Newtonsoft.Json;
using System.Collections.Generic;

namespace QingzhenyunApis.EntityModels
{
    public class OnlineDeviceList
    {
        [JsonProperty(PropertyName = "self")]
        public SelfClient Self { get; set; }

        [JsonProperty(PropertyName = "online")]
        public IList<OnlineClient> Online { get; set; }
    }

}

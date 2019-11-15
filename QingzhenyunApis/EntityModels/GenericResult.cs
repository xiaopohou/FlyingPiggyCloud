using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public class GenericResult<T> : ResultBase
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
    }
}

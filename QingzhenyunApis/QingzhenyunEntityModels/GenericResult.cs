using Newtonsoft.Json;

namespace QingzhenyunApis.QingzhenyunEntityModels
{
    public class GenericResult<T> : ResultBase
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
    }
}

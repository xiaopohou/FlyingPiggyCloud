using Newtonsoft.Json;

namespace SixCloud.Models
{
    public class GenericResult<T> : ResultBase
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
    }
}

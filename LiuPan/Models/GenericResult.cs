using Newtonsoft.Json;

namespace LiuPan.Models
{
    public class GenericResult<T> : ResultBase
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
    }
}

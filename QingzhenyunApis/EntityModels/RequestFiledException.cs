using Newtonsoft.Json;
using System;

namespace QingzhenyunApis.EntityModels
{
    public class RequestFiledException : Exception
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "message")]
        public new string Message { get; set; }

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

}

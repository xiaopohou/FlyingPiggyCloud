using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace QingzhenyunApis.Exceptions
{
    public class RequestFailedException : Exception
    {

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "message")]
        public new string Message { get; set; }

        [JsonProperty(PropertyName = "reference")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        public RequestFailedException() : base()
        {

        }

        public RequestFailedException(string message) : base(message)
        {
        }

        public RequestFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}

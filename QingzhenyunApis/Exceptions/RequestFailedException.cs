using Newtonsoft.Json;
using System;

namespace QingzhenyunApis.Exceptions
{
    public class RequestFailedException : Exception
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


    public sealed class NeedPasswordException : Exception
    {
        internal NeedPasswordException(string message) : base(message) { }
        internal NeedPasswordException(string message, Exception inner) : base(message, inner) { }
    }


    public sealed class UnsupportUrlException : Exception
    {
        internal UnsupportUrlException(string message) : base(message) { }
        internal UnsupportUrlException(string message, Exception inner) : base(message, inner) { }
    }

}

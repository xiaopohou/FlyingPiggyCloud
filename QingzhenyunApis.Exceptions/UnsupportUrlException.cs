using System;

namespace QingzhenyunApis.Exceptions
{
    public sealed class UnsupportUrlException : Exception
    {
        public UnsupportUrlException(string message) : base(message) { }
        public UnsupportUrlException(string message, RequestFailedException inner) : base(message, inner) { }
    }

}

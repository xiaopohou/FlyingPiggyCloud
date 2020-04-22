using System;

namespace QingzhenyunApis.Exceptions
{
    public sealed class UnsupportUrlException : Exception
    {
        internal UnsupportUrlException(string message) : base(message) { }
        internal UnsupportUrlException(string message, RequestFailedException inner) : base(message, inner) { }
    }

}

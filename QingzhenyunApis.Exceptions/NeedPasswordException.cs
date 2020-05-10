using System;

namespace QingzhenyunApis.Exceptions
{
    public sealed class NeedPasswordException : Exception
    {
        public NeedPasswordException(string message) : base(message) { }
        public NeedPasswordException(string message, RequestFailedException inner) : base(message, inner) { }
    }

}

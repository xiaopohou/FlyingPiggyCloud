using System;

namespace QingzhenyunApis.Exceptions
{
    public sealed class NeedPasswordException : Exception
    {
        internal NeedPasswordException(string message) : base(message) { }
        internal NeedPasswordException(string message, RequestFailedException inner) : base(message, inner) { }
    }

}

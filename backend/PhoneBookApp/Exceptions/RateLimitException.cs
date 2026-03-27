using System;

namespace PhoneBookApp.Exceptions;

public class RateLimitException : Exception
{
    public RateLimitException(string message, Exception innerException) 
        : base(message, innerException) { }
}

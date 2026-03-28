using System;

namespace PhoneBookApp.Exceptions;

public class ExternalServiceException : Exception
{
    public ExternalServiceException(string message, Exception innerException) 
        : base(message, innerException) { }
}

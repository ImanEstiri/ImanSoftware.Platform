using ImanSoftware.Framework.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Framework.Core.Exceptions;

/// <summary>
/// Exception thrown when the user is authenticated but not authorized (HTTP 403).
/// </summary>
public class ForbiddenException : FrameworkException
{
    public ForbiddenException(string message = "You do not have permission to access this resource.")
        : base(message, "FORBIDDEN", ErrorType.Failure)
    {
    }

    public ForbiddenException(string message, Exception innerException)
        : base(message, "FORBIDDEN", ErrorType.Failure, innerException)
    {
    }
}
using ImanSoftware.Framework.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Framework.Core.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found (HTTP 404 equivalent).
/// </summary>
public class NotFoundException : FrameworkException
{
    public NotFoundException(string message)
        : base(message, "NOT_FOUND", ErrorType.NotFound)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, "NOT_FOUND", ErrorType.NotFound, innerException)
    {
    }
}
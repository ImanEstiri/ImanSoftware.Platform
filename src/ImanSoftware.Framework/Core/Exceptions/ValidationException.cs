using ImanSoftware.Framework.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Framework.Core.Exceptions;

/// <summary>
/// Exception thrown when validation fails (HTTP 400 with validation errors).
/// </summary>
public class ValidationException : FrameworkException
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(string message, IReadOnlyCollection<string> errors)
        : base(message, "VALIDATION_ERROR", ErrorType.Validation)
    {
        Errors = errors;
    }

    public ValidationException(string message)
        : base(message, "VALIDATION_ERROR", ErrorType.Validation)
    {
        Errors = Array.Empty<string>();
    }

    public ValidationException(string message, Exception innerException)
        : base(message, "VALIDATION_ERROR", ErrorType.Validation, innerException)
    {
        Errors = Array.Empty<string>();
    }
}
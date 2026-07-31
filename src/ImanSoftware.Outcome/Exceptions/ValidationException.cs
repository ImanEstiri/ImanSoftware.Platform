
using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when validation fails (HTTP 400 with validation errors).
/// </summary>
public class ValidationException : FrameworkException
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(string message, IReadOnlyCollection<string> errors)
        : base(message, "VALIDATION_ERROR", OutcomeErrorType.Validation)
    {
        Errors = errors;
    }

    public ValidationException(string message)
        : base(message, "VALIDATION_ERROR", OutcomeErrorType.Validation)
    {
        Errors = Array.Empty<string>();
    }

    public ValidationException(string message, Exception innerException)
        : base(message, "VALIDATION_ERROR", OutcomeErrorType.Validation, innerException)
    {
        Errors = Array.Empty<string>();
    }
}
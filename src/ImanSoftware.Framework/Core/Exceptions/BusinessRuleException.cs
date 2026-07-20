
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Core.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated (HTTP 422 or 400).
/// </summary>
public class BusinessRuleException : FrameworkException
{
    public BusinessRuleException(string message)
        : base(message, "BUSINESS_RULE_VIOLATION", ErrorType.Failure)
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message, "BUSINESS_RULE_VIOLATION", ErrorType.Failure, innerException)
    {
    }
}
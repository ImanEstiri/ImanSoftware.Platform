using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Outcomes;

public class FrameworkExceptionTests
{
    private sealed class TestException : FrameworkException
    {
        public TestException(string message, string errorCode, OutcomeErrorType errorType)
            : base(message, errorCode, errorType)
        {
        }

        public TestException(string message, string errorCode, OutcomeErrorType errorType, Exception innerException)
            : base(message, errorCode, errorType, innerException)
        {
        }
    }

    [Fact]
    public void ToResult_MapsMessageCodeAndTypeIntoFailure()
    {
        var exception = new TestException("failed", "TEST_CODE", OutcomeErrorType.Conflict);

        var outcome = exception.ToResult();

        Assert.True(outcome.IsFailure);
        Assert.Equal("failed", outcome.Error.Message);
        Assert.Equal("TEST_CODE", outcome.Error.Code);
        Assert.Equal(OutcomeErrorType.Conflict, outcome.Error.Type);
    }

    [Fact]
    public void ToResult_Generic_PreservesInnerException()
    {
        var inner = new InvalidOperationException("root cause");
        var exception = new TestException("failed", "TEST_CODE", OutcomeErrorType.NotFound, inner);

        var outcome = exception.ToResult<string>();

        Assert.True(outcome.IsFailure);
        Assert.Equal(OutcomeErrorType.NotFound, outcome.Error.Type);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void NotFoundException_ToResult_UsesNotFoundTypeAndCode()
    {
        var outcome = new NotFoundException("entity missing").ToResult();

        Assert.True(outcome.IsFailure);
        Assert.Equal("entity missing", outcome.Error.Message);
        Assert.Equal("NOT_FOUND", outcome.Error.Code);
        Assert.Equal(OutcomeErrorType.NotFound, outcome.Error.Type);
    }

    [Fact]
    public void BadRequestException_UsesBadRequestCode()
    {
        var exception = new BadRequestException("bad input");

        Assert.Equal("BAD_REQUEST", exception.ErrorCode);
        Assert.Equal(OutcomeErrorType.Failure, exception.Type);
    }

    [Fact]
    public void BusinessRuleException_UsesBusinessRuleViolationCode()
    {
        var exception = new BusinessRuleException("rule broken");

        Assert.Equal("BUSINESS_RULE_VIOLATION", exception.ErrorCode);
        Assert.Equal(OutcomeErrorType.Failure, exception.Type);
    }

    [Fact]
    public void ConflictException_UsesConflictType()
    {
        var exception = new ConflictException("duplicate");

        Assert.Equal("CONFLICT", exception.ErrorCode);
        Assert.Equal(OutcomeErrorType.Conflict, exception.Type);
    }

    [Fact]
    public void ForbiddenException_WithDefaultMessage_SetsPermissionText()
    {
        var exception = new ForbiddenException();

        Assert.Equal("You do not have permission to access this resource.", exception.Message);
        Assert.Equal("FORBIDDEN", exception.ErrorCode);
    }

    [Fact]
    public void UnauthorizedException_WithDefaultMessage_SetsAuthenticationText()
    {
        var exception = new UnauthorizedException();

        Assert.Equal("User is not authenticated.", exception.Message);
        Assert.Equal("UNAUTHORIZED", exception.ErrorCode);
    }

    [Fact]
    public void ValidationException_WithErrors_ExposesErrors()
    {
        var errors = new List<string> { "Name is required.", "Age must be positive." };

        var exception = new ValidationException("validation failed", errors);

        Assert.Equal("VALIDATION_ERROR", exception.ErrorCode);
        Assert.Equal(OutcomeErrorType.Validation, exception.Type);
        Assert.Equal(2, exception.Errors.Count);
        Assert.Contains("Name is required.", exception.Errors);
    }

    [Fact]
    public void ValidationException_MessageOnly_HasEmptyErrors()
    {
        var exception = new ValidationException("validation failed");

        Assert.Empty(exception.Errors);
    }

    [Fact]
    public void ValidationException_WithInnerException_HasEmptyErrors()
    {
        var exception = new ValidationException("validation failed", new ArgumentException("inner"));

        Assert.Empty(exception.Errors);
        Assert.IsType<ArgumentException>(exception.InnerException);
    }
}

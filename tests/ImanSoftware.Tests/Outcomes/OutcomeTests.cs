using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Outcomes;

public class OutcomeTests
{
    [Fact]
    public void Successful_ReturnsSuccessWithNoneError()
    {
        var outcome = Outcome.Successful();

        Assert.True(outcome.IsSuccess);
        Assert.False(outcome.IsFailure);
        Assert.Equal(OutcomeError.None, outcome.Error);
    }

    [Fact]
    public void Successful_WithData_ReturnsSuccessAndExposesData()
    {
        var outcome = Outcome.Successful(42);

        Assert.True(outcome.IsSuccess);
        Assert.Equal(42, outcome.Data);
    }

    [Fact]
    public void Failure_ReturnsFailureWithError()
    {
        var error = OutcomeError.Create("boom", "BOOM", OutcomeErrorType.Failure);

        var outcome = Outcome.Failure(error);

        Assert.True(outcome.IsFailure);
        Assert.False(outcome.IsSuccess);
        Assert.Equal(error, outcome.Error);
    }

    [Fact]
    public void Failure_Generic_ReturnsFailureWithDefaultData()
    {
        var outcome = Outcome.Failure<string>(OutcomeError.DefaultError);

        Assert.True(outcome.IsFailure);
        Assert.Equal(OutcomeError.DefaultError, outcome.Error);
    }

    [Fact]
    public void Data_WhenFailed_ThrowsInvalidOperationException()
    {
        var outcome = Outcome.Failure<string>(OutcomeError.Create("x", "X", OutcomeErrorType.Failure));

        var exception = Assert.Throws<InvalidOperationException>(() => _ = outcome.Data);
        Assert.Equal("Cannot access Data of a failed result. Check IsSuccess first.", exception.Message);
    }

    [Fact]
    public void Data_WhenSuccessfulButNull_ThrowsInvalidOperationException()
    {
        var outcome = Outcome.Successful<string>(null!);

        Assert.True(outcome.IsSuccess);

        var exception = Assert.Throws<InvalidOperationException>(() => _ = outcome.Data);
        Assert.Equal("Cannot access Data of a failed result. Check IsSuccess first.", exception.Message);
    }

    [Fact]
    public void Data_WithValueTypeSuccessfulZero_ReturnsZero()
    {
        var outcome = Outcome.Successful(0);

        Assert.True(outcome.IsSuccess);
        Assert.Equal(0, outcome.Data);
    }

    [Fact]
    public void ImplicitOperator_FromNonNullValue_ReturnsSuccess()
    {
        Outcome<string> outcome = "value";

        Assert.True(outcome.IsSuccess);
        Assert.Equal("value", outcome.Data);
    }

    [Fact]
    public void ImplicitOperator_FromNull_ReturnsFailureWithProblemType()
    {
        string? value = null;

        Outcome<string> outcome = value;

        Assert.True(outcome.IsFailure);
        Assert.Equal(OutcomeErrorType.Problem, outcome.Error.Type);
        Assert.Equal(string.Empty, outcome.Error.Message);
        Assert.Equal(string.Empty, outcome.Error.Code);
    }

    [Fact]
    public void ImplicitOperator_FromBoxedValueType_ReturnsSuccess()
    {
        Outcome<int> outcome = 5;

        Assert.True(outcome.IsSuccess);
        Assert.Equal(5, outcome.Data);
    }

    [Fact]
    public void Ctor_DoesNotValidateSuccessErrorConsistency()
    {
        var outcome = new Outcome(true, OutcomeError.DefaultError);

        Assert.True(outcome.IsSuccess);
        Assert.Equal(OutcomeError.DefaultError, outcome.Error);
    }
}

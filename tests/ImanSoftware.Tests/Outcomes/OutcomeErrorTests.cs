using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.Outcomes;

public class OutcomeErrorTests
{
    [Fact]
    public void Ctor_SetsMessageCodeAndType()
    {
        var error = new OutcomeError("message", "CODE", OutcomeErrorType.NotFound);

        Assert.Equal("message", error.Message);
        Assert.Equal("CODE", error.Code);
        Assert.Equal(OutcomeErrorType.NotFound, error.Type);
    }

    [Fact]
    public void Create_ReturnsErrorWithGivenValues()
    {
        var error = OutcomeError.Create("msg", "C1", OutcomeErrorType.Validation);

        Assert.Equal("msg", error.Message);
        Assert.Equal("C1", error.Code);
        Assert.Equal(OutcomeErrorType.Validation, error.Type);
    }

    [Fact]
    public void ExceptionCtor_TakesExceptionMessageAndFailureType()
    {
        var error = new OutcomeError(new InvalidOperationException("inner boom"));

        Assert.Equal("inner boom", error.Message);
        Assert.Equal(string.Empty, error.Code);
        Assert.Equal(OutcomeErrorType.Failure, error.Type);
    }

    [Fact]
    public void ExceptionCtor_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OutcomeError(null!));
    }

    [Fact]
    public void DefaultError_HasKnownMessageCodeAndType()
    {
        Assert.Equal("Somthing went Wrong . . .", OutcomeError.DefaultError.Message);
        Assert.Equal(string.Empty, OutcomeError.DefaultError.Code);
        Assert.Equal(OutcomeErrorType.Failure, OutcomeError.DefaultError.Type);
    }

    [Fact]
    public void None_HasEmptyMessageAndCodeWithNoneType()
    {
        Assert.Equal(string.Empty, OutcomeError.None.Message);
        Assert.Equal(string.Empty, OutcomeError.None.Code);
        Assert.Equal(OutcomeErrorType.None, OutcomeError.None.Type);
    }

    [Fact]
    public void RecordEquality_MembersWithSameValuesAreEqual()
    {
        var first = new OutcomeError("m", "c", OutcomeErrorType.Conflict);
        var second = new OutcomeError("m", "c", OutcomeErrorType.Conflict);

        Assert.Equal(first, second);
    }

    [Fact]
    public void RecordEquality_DifferentTypeIsNotEqual()
    {
        var first = new OutcomeError("m", "c", OutcomeErrorType.Conflict);
        var second = new OutcomeError("m", "c", OutcomeErrorType.NotFound);

        Assert.NotEqual(first, second);
    }
}

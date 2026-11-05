using ImanSoftware.Extensions;

namespace ImanSoftware.Tests.Extensions;

public class ExceptionExtensionsTests
{
    [Fact]
    public void GetFullMessage_WithInnerExceptions_AppendsEachMessageOnItsOwnLine()
    {
        var exception = new InvalidOperationException("outer", new ArgumentException("inner"));

        var fullMessage = exception.GetFullMessage();

        Assert.Contains("outer", fullMessage, StringComparison.Ordinal);
        Assert.Contains("inner", fullMessage, StringComparison.Ordinal);
        Assert.StartsWith("outer", fullMessage, StringComparison.Ordinal);
        Assert.EndsWith("inner" + Environment.NewLine, fullMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void GetFullMessage_WithNull_ReturnsEmpty()
    {
        Exception? exception = null;

        Assert.Equal(string.Empty, exception.GetFullMessage());
    }

    [Fact]
    public void GetFullStackTrace_WithNull_ReturnsEmpty()
    {
        Exception? exception = null;

        Assert.Equal(string.Empty, exception.GetFullStackTrace());
    }

    [Fact]
    public void Is_ForMatchingType_ReturnsTrue()
    {
        Exception exception = new ArgumentException("bad argument");

        Assert.True(exception.Is<ArgumentException>());
    }

    [Fact]
    public void Is_ForDerivedType_ReturnsTrue()
    {
        Exception exception = new ArgumentOutOfRangeException("param");

        Assert.True(exception.Is<ArgumentException>());
    }

    [Fact]
    public void Is_ForUnrelatedType_ReturnsFalse()
    {
        Exception exception = new ArgumentException("bad argument");

        Assert.False(exception.Is<InvalidOperationException>());
    }

    [Fact]
    public void GetInnermostException_WalksToDeepestInner()
    {
        var innermost = new InvalidOperationException("root");
        var exception = new Exception("level1", new Exception("level2", innermost));

        Assert.Same(innermost, exception.GetInnermostException());
    }

    [Fact]
    public void GetInnermostException_WithoutInner_ReturnsSelf()
    {
        var exception = new Exception("only");

        Assert.Same(exception, exception.GetInnermostException());
    }
}

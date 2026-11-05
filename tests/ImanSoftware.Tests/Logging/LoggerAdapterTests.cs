using Microsoft.Extensions.Logging;
using ImanSoftware.Logging;

namespace ImanSoftware.Tests.Logging;

public class LoggerAdapterTests
{
    private sealed class SampleService;

    private sealed class RecordingLogger : ILogger
    {
        public sealed record Entry(LogLevel Level, string Message, Exception? Exception);

        public List<Entry> Entries { get; } = new();
        public HashSet<LogLevel> EnabledLevels { get; } =
            [LogLevel.Trace, LogLevel.Debug, LogLevel.Information, LogLevel.Warning, LogLevel.Error, LogLevel.Critical];
        public IDisposable? Scope { get; set; } = new DummyScope();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => Scope;

        public bool IsEnabled(LogLevel logLevel) => EnabledLevels.Contains(logLevel);

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add(new Entry(logLevel, formatter(state, exception), exception));

        public sealed class DummyScope : IDisposable
        {
            public void Dispose() { }
        }
    }

    private sealed class RecordingLoggerFactory : ILoggerFactory
    {
        public RecordingLogger Logger { get; } = new();

        public ILogger CreateLogger(string categoryName) => Logger;
        public void AddProvider(ILoggerProvider provider) { }
        public void Dispose() { }
    }

    private static (LoggerAdapter<SampleService> Adapter, RecordingLogger Inner) CreateAdapter()
    {
        var factory = new RecordingLoggerFactory();
        return (new LoggerAdapter<SampleService>(factory), factory.Logger);
    }

    [Fact]
    public void CategoryName_UsesTypeFullName()
    {
        var (adapter, _) = CreateAdapter();

        Assert.Equal(typeof(SampleService).FullName, adapter.CategoryName);
    }

    [Fact]
    public void IsEnabled_DelegatesToInnerLogger()
    {
        var (adapter, inner) = CreateAdapter();
        inner.EnabledLevels.Remove(LogLevel.Warning);

        Assert.True(adapter.IsEnabled(LogLevel.Information));
        Assert.False(adapter.IsEnabled(LogLevel.Warning));
    }

    [Theory]
    [InlineData(nameof(LoggerAdapter<SampleService>.LogTrace), LogLevel.Trace)]
    [InlineData(nameof(LoggerAdapter<SampleService>.LogDebug), LogLevel.Debug)]
    [InlineData(nameof(LoggerAdapter<SampleService>.LogInformation), LogLevel.Information)]
    [InlineData(nameof(LoggerAdapter<SampleService>.LogWarning), LogLevel.Warning)]
    [InlineData(nameof(LoggerAdapter<SampleService>.LogCritical), LogLevel.Critical)]
    public void LogMethods_MapToMatchingLevel(string method, LogLevel expectedLevel)
    {
        var (adapter, inner) = CreateAdapter();

        typeof(LoggerAdapter<SampleService>)
            .GetMethod(method, [typeof(string), typeof(object?[])])!
            .Invoke(adapter, ["hello", Array.Empty<object?>()]);

        var entry = Assert.Single(inner.Entries);
        Assert.Equal(expectedLevel, entry.Level);
        Assert.Equal("hello", entry.Message);
    }

    [Fact]
    public void LogInformation_FormatsMessageWithArgs()
    {
        var (adapter, inner) = CreateAdapter();

        adapter.LogInformation("User {Id} logged in", 42);

        Assert.Equal("User 42 logged in", Assert.Single(inner.Entries).Message);
    }

    [Fact]
    public void LogError_WithException_CapturesException()
    {
        var (adapter, inner) = CreateAdapter();
        var exception = new InvalidOperationException("boom");

        adapter.LogError(exception, "failed with {Code}", 500);

        var entry = Assert.Single(inner.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(exception, entry.Exception);
        Assert.Equal("failed with 500", entry.Message);
    }

    [Fact]
    public void Log_UsesGivenLevel()
    {
        var (adapter, inner) = CreateAdapter();

        adapter.Log(LogLevel.Warning, "custom");

        Assert.Equal(LogLevel.Warning, Assert.Single(inner.Entries).Level);
    }

    [Fact]
    public void LogInformation_WhenLevelDisabled_DoesNotCallInnerLogger()
    {
        var (adapter, inner) = CreateAdapter();
        inner.EnabledLevels.Remove(LogLevel.Information);

        adapter.LogInformation("hidden");

        Assert.Empty(inner.Entries);
    }

    [Fact]
    public void LogError_WhenLevelDisabled_DoesNotCallInnerLogger()
    {
        var (adapter, inner) = CreateAdapter();
        inner.EnabledLevels.Remove(LogLevel.Error);

        adapter.LogError(new Exception("boom"), "hidden");

        Assert.Empty(inner.Entries);
    }

    [Fact]
    public void BeginScope_PassesThroughToInnerLogger()
    {
        var (adapter, inner) = CreateAdapter();
        var scope = new RecordingLogger.DummyScope();
        inner.Scope = scope;

        Assert.Same(scope, adapter.BeginScope("scope-state"));
    }
}

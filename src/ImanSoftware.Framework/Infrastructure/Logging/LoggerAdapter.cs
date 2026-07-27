using ImanSoftware.Framework.Abstractions.Logging;
using Microsoft.Extensions.Logging;

namespace ImanSoftware.Framework.Infrastructure.Logging;

/// <summary>
/// Default implementation of ILoggerAdapter using Microsoft.Extensions.Logging.ILogger.
/// </summary>
public class LoggerAdapter<TCategoryName> : ILoggerAdapter<TCategoryName>
{
    private readonly ILogger _logger;

    public string CategoryName => typeof(TCategoryName).FullName ?? typeof(TCategoryName).Name;

    public LoggerAdapter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(CategoryName);
    }

    public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

    public void LogInformation(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
            _logger.LogWarning(message, args);
    }

    public void LogError(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Error))
            _logger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Error))
            _logger.LogError(exception, message, args);
    }

    public void LogDebug(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(message, args);
    }

    public void LogTrace(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
            _logger.LogTrace(message, args);
    }

    public void LogCritical(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Critical))
            _logger.LogCritical(message, args);
    }

    public void Log(LogLevel logLevel, string message, params object?[] args)
    {
        if (_logger.IsEnabled(logLevel))
            _logger.Log(logLevel, message, args);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope(state);
    }
}
using Microsoft.Extensions.Logging;

namespace ImanSoftware.Logging;

/// <summary>
/// Contract for a structured logger adapter.
/// Provides a unified abstraction over Microsoft.Extensions.Logging and other logging frameworks.
/// </summary>
/// <typeparam name="TCategoryName">The type whose name is used for the logger category name.</typeparam>
public interface ILoggerAdapter<out TCategoryName>
{
    /// <summary>
    /// Gets the category name of the logger.
    /// </summary>
    string CategoryName { get; }

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    bool IsEnabled(LogLevel logLevel);

    /// <summary>
    /// Logs a message with Information level.
    /// </summary>
    void LogInformation(string message, params object?[] args);

    /// <summary>
    /// Logs a message with Warning level.
    /// </summary>
    void LogWarning(string message, params object?[] args);

    /// <summary>
    /// Logs a message with Error level.
    /// </summary>
    void LogError(string message, params object?[] args);

    /// <summary>
    /// Logs an exception with Error level.
    /// </summary>
    void LogError(Exception exception, string message, params object?[] args);

    /// <summary>
    /// Logs a message with Debug level.
    /// </summary>
    void LogDebug(string message, params object?[] args);

    /// <summary>
    /// Logs a message with Trace level.
    /// </summary>
    void LogTrace(string message, params object?[] args);

    /// <summary>
    /// Logs a message with Critical level.
    /// </summary>
    void LogCritical(string message, params object?[] args);

    /// <summary>
    /// Logs a message with the specified log level.
    /// </summary>
    void Log(LogLevel logLevel, string message, params object?[] args);

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    IDisposable? BeginScope<TState>(TState state) where TState : notnull;
}
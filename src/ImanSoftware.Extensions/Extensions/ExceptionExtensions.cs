// Core/Extensions/ExceptionExtensions.cs
using System.Text;

namespace ImanSoftware.Extensions;

public static class ExceptionExtensions
{
    /// <summary>
    /// Gets the full error message including inner exceptions.
    /// </summary>
    public static string GetFullMessage(this Exception ex)
    {
        if (ex is null)
            return string.Empty;

        var sb = new StringBuilder();
        var current = ex;
        while (current is not null)
        {
            sb.AppendLine(current.Message);
            current = current.InnerException;
        }
        return sb.ToString();
    }

    /// <summary>
    /// Gets the full stack trace including inner exceptions.
    /// </summary>
    public static string GetFullStackTrace(this Exception ex)
    {
        if (ex is null)
            return string.Empty;

        var sb = new StringBuilder();
        var current = ex;
        while (current is not null)
        {
            sb.AppendLine(current.StackTrace);
            current = current.InnerException;
        }
        return sb.ToString();
    }

    /// <summary>
    /// Checks if an exception is of a specific type or its derived types.
    /// </summary>
    public static bool Is<TException>(this Exception ex) where TException : Exception
        => ex is TException;

    /// <summary>
    /// Recursively finds the most inner exception.
    /// </summary>
    public static Exception GetInnermostException(this Exception ex)
    {
        var current = ex;
        while (current.InnerException is not null)
            current = current.InnerException;
        return current;
    }
}
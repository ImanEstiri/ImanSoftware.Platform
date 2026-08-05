// مسیر: Abstractions/ICommand.cs
using MediatR;

namespace ImanSoftware.CQRS;

/// <summary>
/// Base interface for a command (without a response).
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Base interface for a command with a response.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
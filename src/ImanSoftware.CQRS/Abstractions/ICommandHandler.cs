// مسیر: Abstractions/ICommandHandler.cs
using MediatR;

namespace ImanSoftware.CQRS;

/// <summary>
/// Base interface for a command handler (without response).
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

/// <summary>
/// Base interface for a command handler with a response.
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}
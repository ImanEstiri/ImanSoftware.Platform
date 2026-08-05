// مسیر: Infrastructure/MediatorExtensions.cs
using MediatR;

namespace ImanSoftware.CQRS;

public static class MediatorExtensions
{
    /// <summary>
    /// Sends a command without a response.
    /// </summary>
    public static async Task SendCommandAsync<TCommand>(
        this IMediator mediator,
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        await mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Sends a command and returns a response.
    /// </summary>
    public static async Task<TResponse> SendCommandAsync<TCommand, TResponse>(
        this IMediator mediator,
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>
    {
        return await mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Sends a query and returns a response.
    /// </summary>
    public static async Task<TResponse> SendQueryAsync<TQuery, TResponse>(
        this IMediator mediator,
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>
    {
        return await mediator.Send(query, cancellationToken);
    }
}
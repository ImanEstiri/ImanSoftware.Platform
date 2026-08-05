// مسیر: Abstractions/IQueryHandler.cs
using MediatR;

namespace ImanSoftware.CQRS;

/// <summary>
/// Base interface for a query handler.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
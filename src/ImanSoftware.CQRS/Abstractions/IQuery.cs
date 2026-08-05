// مسیر: Abstractions/IQuery.cs
using MediatR;

namespace ImanSoftware.CQRS;

/// <summary>
/// Base interface for a query.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
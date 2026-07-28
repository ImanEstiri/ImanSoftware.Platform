
using System.Data;

namespace ImanSoftware.Framework.Abstractions.DataAccess;

/// <summary>
/// Represents a database transaction scope.
/// </summary>
public interface ITransactionScope : IDisposable
{
    IDbTransaction Transaction { get; }
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
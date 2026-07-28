// Abstractions/DataAccess/IRepository.cs

namespace ImanSoftware.Framework.Abstractions.DataAccess;

using ImanSoftware.Framework.Core.Results;

using System.Data;


/// <summary>
/// Contract for SQL data access operations.
/// Supports both stored procedures and ad-hoc queries.
/// </summary>
public interface ISqlDataAccessor
{
    /// <summary>
    /// Executes a query and returns a single result.
    /// </summary>
    Task<Result<T>> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query and returns a collection of results.
    /// </summary>
    Task<Result<IEnumerable<T>>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a non-query command and returns the number of affected rows.
    /// </summary>
    Task<Result<int>> ExecuteAsync(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query and returns a scalar value.
    /// </summary>
    Task<Result<T>> ExecuteScalarAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    Task<Result<ITransactionScope>> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the connection factory.
    /// </summary>
    IDbConnectionFactory ConnectionFactory { get; }
}
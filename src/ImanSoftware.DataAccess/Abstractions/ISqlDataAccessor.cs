// Abstractions/DataAccess/IRepository.cs

namespace ImanSoftware.DataAccess;

using ImanSoftware.Outcomes;

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
    Task<Outcome<T>> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query and returns a collection of results.
    /// </summary>
    Task<Outcome<IEnumerable<T>>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a non-query command and returns the number of affected rows.
    /// </summary>
    Task<Outcome<int>> ExecuteAsync(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query and returns a scalar value.
    /// </summary>
    Task<Outcome<T>> ExecuteScalarAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    Task<Outcome<ITransactionScope>> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the connection factory.
    /// </summary>
    IDbConnectionFactory ConnectionFactory { get; }
}
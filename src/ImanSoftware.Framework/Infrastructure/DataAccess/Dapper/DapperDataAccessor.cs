// مسیر: Infrastructure/DataAccess/Dapper/DapperDataAccessor.cs
using System.Data;
using Dapper;
using ImanSoftware.Framework.Abstractions.DataAccess;
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Infrastructure.DataAccess.Dapper;

/// <summary>
/// Default implementation of ISqlDataAccessor using Dapper.
/// </summary>
public class DapperDataAccessor : ISqlDataAccessor
{
    public IDbConnectionFactory ConnectionFactory { get; }

    public DapperDataAccessor(IDbConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    public async Task<Result<T>> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QuerySingleOrDefaultAsync<T>(
                sql,
                parameters,
                transaction,
                commandType: commandType);

            return Result<T>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new AppError(ex.Message, "DB_QUERY_ERROR", ErrorType.Failure);
            return Result.Failure<T>(error);
        }
    }

    public async Task<Result<IEnumerable<T>>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<T>(
                sql,
                parameters,
                transaction,
                commandType: commandType);

            return Result<IEnumerable<T>>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new AppError(ex.Message, "DB_QUERY_ERROR", ErrorType.Failure);
            return Result.Failure<IEnumerable<T>>(error);
        }
    }

    public async Task<Result<int>> ExecuteAsync(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var affectedRows = await connection.ExecuteAsync(
                sql,
                parameters,
                transaction,
                commandType: commandType);

            return Result<int>.Successful(affectedRows);
        }
        catch (Exception ex)
        {
            var error = new AppError(ex.Message, "DB_EXECUTE_ERROR", ErrorType.Failure);
            return Result.Failure<int>(error);
        }
    }

    public async Task<Result<T>> ExecuteScalarAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.ExecuteScalarAsync<T>(
                sql,
                parameters,
                transaction,
                commandType: commandType);

            return Result<T>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new AppError(ex.Message, "DB_SCALAR_ERROR", ErrorType.Failure);
            return Result.Failure<T>(error);
        }
    }

    public async Task<Result<ITransactionScope>> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var transactionScope = new DapperTransactionScope(connection, isolationLevel);
            return Result.Successful<ITransactionScope>(transactionScope);
        }
        catch (Exception ex)
        {
            var error = new AppError(ex.Message, "DB_TRANSACTION_ERROR", ErrorType.Failure);
            return Result.Failure<ITransactionScope>(error);
        }
    }
}
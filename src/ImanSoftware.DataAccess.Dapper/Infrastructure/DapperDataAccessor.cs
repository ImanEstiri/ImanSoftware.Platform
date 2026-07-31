// مسیر: Infrastructure/DataAccess/Dapper/DapperDataAccessor.cs
using System.Data;
using Dapper;
using ImanSoftware.Outcomes;

namespace ImanSoftware.DataAccess.Dapper;

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

    public async Task<Outcome<T>> QuerySingleAsync<T>(
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

            return Outcome<T>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_QUERY_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<T>(error);
        }
    }

    public async Task<Outcome<IEnumerable<T>>> QueryAsync<T>(
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

            return Outcome<IEnumerable<T>>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_QUERY_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<IEnumerable<T>>(error);
        }
    }

    public async Task<Outcome<int>> ExecuteAsync(
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

            return Outcome<int>.Successful(affectedRows);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_EXECUTE_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<int>(error);
        }
    }

    public async Task<Outcome<T>> ExecuteScalarAsync<T>(
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

            return Outcome<T>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_SCALAR_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<T>(error);
        }
    }

    public async Task<Outcome<ITransactionScope>> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var transactionScope = new DapperTransactionScope(connection, isolationLevel);
            return Outcome.Successful<ITransactionScope>(transactionScope);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_TRANSACTION_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<ITransactionScope>(error);
        }
    }
}
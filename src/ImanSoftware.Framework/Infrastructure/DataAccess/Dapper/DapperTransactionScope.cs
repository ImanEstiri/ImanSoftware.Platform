// مسیر: Infrastructure/DataAccess/Dapper/DapperTransactionScope.cs
using System.Data;
using ImanSoftware.Framework.Abstractions.DataAccess;

namespace ImanSoftware.Framework.Infrastructure.DataAccess.Dapper;

/// <summary>
/// Default implementation of ITransactionScope.
/// </summary>
public class DapperTransactionScope : ITransactionScope
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public IDbTransaction Transaction => _transaction ?? throw new InvalidOperationException("Transaction has not been started.");

    public DapperTransactionScope(IDbConnection connection, IsolationLevel isolationLevel)
    {
        _connection = connection;
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        _transaction = _connection.BeginTransaction(isolationLevel);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction is null.");

        await Task.Run(() => _transaction.Commit(), cancellationToken);
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction is null.");

        await Task.Run(() => _transaction.Rollback(), cancellationToken);
        _transaction.Dispose();
        _transaction = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
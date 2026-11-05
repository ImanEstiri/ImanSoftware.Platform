using System.Data;
using NSubstitute;
using ImanSoftware.DataAccess.Dapper;

namespace ImanSoftware.Tests.DataAccess.Dapper;

public class DapperTransactionScopeTests
{
    private readonly IDbConnection _connection = Substitute.For<IDbConnection>();
    private readonly IDbTransaction _transaction = Substitute.For<IDbTransaction>();

    private DapperTransactionScope CreateScope(ConnectionState connectionState = ConnectionState.Closed)
    {
        _connection.State.Returns(connectionState);
        _connection.BeginTransaction(IsolationLevel.ReadCommitted).Returns(_transaction);
        return new DapperTransactionScope(_connection, IsolationLevel.ReadCommitted);
    }

    [Fact]
    public void Ctor_WithClosedConnection_OpensConnectionAndBeginsTransaction()
    {
        CreateScope();

        _connection.Received(1).Open();
        _connection.Received(1).BeginTransaction(IsolationLevel.ReadCommitted);
    }

    [Fact]
    public void Ctor_WithOpenConnection_DoesNotOpenAgain()
    {
        CreateScope(ConnectionState.Open);

        _connection.DidNotReceive().Open();
    }

    [Fact]
    public void Transaction_AfterConstruction_ReturnsUnderlyingTransaction()
    {
        var scope = CreateScope();

        Assert.Same(_transaction, scope.Transaction);
    }

    [Fact]
    public async Task CommitAsync_CommitsAndDisposesTransaction()
    {
        var scope = CreateScope();

        await scope.CommitAsync();

        _transaction.Received(1).Commit();
        _transaction.Received(1).Dispose();
    }

    [Fact]
    public async Task CommitAsync_AfterCommit_TransactionGetterThrows()
    {
        var scope = CreateScope();
        await scope.CommitAsync();

        var exception = Assert.Throws<InvalidOperationException>(() => _ = scope.Transaction);
        Assert.Equal("Transaction has not been started.", exception.Message);
    }

    [Fact]
    public async Task CommitAsync_CalledTwice_ThrowsInvalidOperationException()
    {
        var scope = CreateScope();
        await scope.CommitAsync();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => scope.CommitAsync());
        Assert.Equal("Transaction is null.", exception.Message);
    }

    [Fact]
    public async Task RollbackAsync_RollbacksAndDisposesTransaction()
    {
        var scope = CreateScope();

        await scope.RollbackAsync();

        _transaction.Received(1).Rollback();
        _transaction.Received(1).Dispose();
    }

    [Fact]
    public async Task RollbackAsync_CalledTwice_ThrowsInvalidOperationException()
    {
        var scope = CreateScope();
        await scope.RollbackAsync();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => scope.RollbackAsync());
        Assert.Equal("Transaction is null.", exception.Message);
    }

    [Fact]
    public async Task CommitAfterRollback_ThrowsInvalidOperationException()
    {
        var scope = CreateScope();
        await scope.RollbackAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => scope.CommitAsync());
    }

    [Fact]
    public void Dispose_DisposesTransactionAndConnection()
    {
        var scope = CreateScope();

        scope.Dispose();

        _transaction.Received(1).Dispose();
        _connection.Received(1).Dispose();
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var scope = CreateScope();

        scope.Dispose();
        scope.Dispose();

        _connection.Received(1).Dispose();
    }

    [Fact]
    public void Dispose_AfterCommit_DoesNotThrow()
    {
        var scope = CreateScope();

        scope.CommitAsync().GetAwaiter().GetResult();
        scope.Dispose();

        _connection.Received(1).Dispose();
    }
}

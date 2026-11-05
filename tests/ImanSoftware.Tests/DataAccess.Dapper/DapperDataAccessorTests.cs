using System.Data;
using Microsoft.Data.Sqlite;
using ImanSoftware.DataAccess;
using ImanSoftware.DataAccess.Dapper;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.DataAccess.Dapper;

public class DapperDataAccessorTests
{
    private const string DatabaseName = "imandappertestdb";

    private sealed class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private sealed class ThrowingConnectionFactory : IDbConnectionFactory
    {
        public string ConnectionString => "throwing";
        public IDbConnection CreateConnection() => throw new InvalidOperationException("factory failure");
    }

    private static (SqliteConnection MasterConnection, DapperDataAccessor Accessor) CreateFixture()
    {
        var masterConnection = new SqliteConnection($"Data Source=file:{DatabaseName}?mode=memory&cache=shared");
        masterConnection.Open();

        var accessor = new DapperDataAccessor(
            new SharedSqliteConnectionFactory($"Data Source=file:{DatabaseName}?mode=memory&cache=shared"));

        return (masterConnection, accessor);
    }

    private sealed class SharedSqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SharedSqliteConnectionFactory(string connectionString) => _connectionString = connectionString;

        public string ConnectionString => _connectionString;

        public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
    }

    [Fact]
    public void Ctor_SetsConnectionFactory()
    {
        var factory = new ThrowingConnectionFactory();

        Assert.Same(factory, new DapperDataAccessor(factory).ConnectionFactory);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesTable_ReturnsAffectedRows()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;
        var table = "t_execute_" + Guid.NewGuid().ToString("N");

        var createResult = await accessor.ExecuteAsync($"CREATE TABLE {table} (Id INTEGER PRIMARY KEY, Name TEXT)");
        var insertResult = await accessor.ExecuteAsync($"INSERT INTO {table} (Id, Name) VALUES (1, 'iman'), (2, 'test')");

        Assert.True(createResult.IsSuccess);
        Assert.Equal(2, insertResult.Data);
    }

    [Fact]
    public async Task QueryAsync_ReturnsAllRows()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;
        var table = "t_query_" + Guid.NewGuid().ToString("N");
        await accessor.ExecuteAsync($"CREATE TABLE {table} (Id INTEGER PRIMARY KEY, Name TEXT)");
        await accessor.ExecuteAsync($"INSERT INTO {table} (Id, Name) VALUES (1, @Name)", new { Name = "first" });
        await accessor.ExecuteAsync($"INSERT INTO {table} (Id, Name) VALUES (2, @Name)", new { Name = "second" });

        var result = await accessor.QueryAsync<Person>($"SELECT Id, Name FROM {table} ORDER BY Id");

        Assert.True(result.IsSuccess);
        var rows = result.Data.ToList();
        Assert.Equal(2, rows.Count);
        Assert.Equal("first", rows[0].Name);
        Assert.Equal("second", rows[1].Name);
    }

    [Fact]
    public async Task QuerySingleAsync_ReturnsMatchingRow()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;
        var table = "t_single_" + Guid.NewGuid().ToString("N");
        await accessor.ExecuteAsync($"CREATE TABLE {table} (Id INTEGER PRIMARY KEY, Name TEXT)");
        await accessor.ExecuteAsync($"INSERT INTO {table} VALUES (1, @Name)", new { Name = "only" });

        var result = await accessor.QuerySingleAsync<Person>($"SELECT Id, Name FROM {table} WHERE Id = 1");

        Assert.True(result.IsSuccess);
        Assert.Equal("only", result.Data.Name);
    }

    [Fact]
    public async Task QuerySingleAsync_WithNoRow_ReturnsSuccessButDataAccessThrows()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;
        var table = "t_empty_" + Guid.NewGuid().ToString("N");
        await accessor.ExecuteAsync($"CREATE TABLE {table} (Id INTEGER PRIMARY KEY, Name TEXT)");

        var result = await accessor.QuerySingleAsync<Person>($"SELECT Id, Name FROM {table} WHERE Id = 999");

        // Known trap: success with null data — Data getter still throws.
        Assert.True(result.IsSuccess);
        var exception = Assert.Throws<InvalidOperationException>(() => _ = result.Data);
        Assert.Equal("Cannot access Data of a failed result. Check IsSuccess first.", exception.Message);
    }

    [Fact]
    public async Task ExecuteScalarAsync_ReturnsScalarValue()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;
        var table = "t_scalar_" + Guid.NewGuid().ToString("N");
        await accessor.ExecuteAsync($"CREATE TABLE {table} (Id INTEGER PRIMARY KEY, Name TEXT)");
        await accessor.ExecuteAsync($"INSERT INTO {table} VALUES (1, 'a'), (2, 'b')");

        var result = await accessor.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {table}");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data);
    }

    [Fact]
    public async Task QueryAsync_WithMissingTable_ReturnsFailureWithDbQueryErrorCode()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;

        var result = await accessor.QueryAsync<Person>("SELECT Id, Name FROM missing_table");

        Assert.True(result.IsFailure);
        Assert.Equal("DB_QUERY_ERROR", result.Error.Code);
        Assert.Equal(OutcomeErrorType.Failure, result.Error.Type);
        Assert.Contains("no such table", result.Error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task QuerySingleAsync_WithMissingTable_ReturnsFailureWithDbQueryErrorCode()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;

        var result = await accessor.QuerySingleAsync<Person>("SELECT Id, Name FROM missing_table");

        Assert.True(result.IsFailure);
        Assert.Equal("DB_QUERY_ERROR", result.Error.Code);
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingTable_ReturnsFailureWithDbExecuteErrorCode()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;

        var result = await accessor.ExecuteAsync("INSERT INTO missing_table VALUES (1)");

        Assert.True(result.IsFailure);
        Assert.Equal("DB_EXECUTE_ERROR", result.Error.Code);
    }

    [Fact]
    public async Task ExecuteScalarAsync_WithMissingTable_ReturnsFailureWithDbScalarErrorCode()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;

        var result = await accessor.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM missing_table");

        Assert.True(result.IsFailure);
        Assert.Equal("DB_SCALAR_ERROR", result.Error.Code);
    }

    [Fact]
    public async Task QueryAsync_WhenFactoryThrows_ReturnsFailureWithFactoryMessage()
    {
        var accessor = new DapperDataAccessor(new ThrowingConnectionFactory());

        var result = await accessor.QueryAsync<Person>("SELECT 1");

        Assert.True(result.IsFailure);
        Assert.Equal("DB_QUERY_ERROR", result.Error.Code);
        Assert.Equal("factory failure", result.Error.Message);
    }

    [Fact]
    public async Task BeginTransactionAsync_ReturnsSuccessfulTransactionScope()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;
        var table = "t_tx_" + Guid.NewGuid().ToString("N");
        await accessor.ExecuteAsync($"CREATE TABLE {table} (Id INTEGER PRIMARY KEY, Name TEXT)");

        var result = await accessor.BeginTransactionAsync();

        Assert.True(result.IsSuccess);
        Assert.IsAssignableFrom<ITransactionScope>(result.Data);
        await result.Data.CommitAsync();
    }

    [Fact]
    public async Task BeginTransactionAsync_WhenFactoryThrows_ReturnsFailureWithDbTransactionErrorCode()
    {
        var accessor = new DapperDataAccessor(new ThrowingConnectionFactory());

        var result = await accessor.BeginTransactionAsync();

        Assert.True(result.IsFailure);
        Assert.Equal("DB_TRANSACTION_ERROR", result.Error.Code);
        Assert.Equal("factory failure", result.Error.Message);
    }

    [Fact]
    public async Task RollbackAsync_CompletesWithoutError()
    {
        var (master, accessor) = CreateFixture();
        using var masterLease = master;
        var table = "t_rb_" + Guid.NewGuid().ToString("N");
        await accessor.ExecuteAsync($"CREATE TABLE {table} (Id INTEGER PRIMARY KEY, Name TEXT)");

        var tx = await accessor.BeginTransactionAsync();

        Assert.True(tx.IsSuccess);
        await tx.Data.RollbackAsync();
    }
}

using System.Data;
using Microsoft.Data.SqlClient;
using ImanSoftware.DataAccess.Dapper;

namespace ImanSoftware.Tests.DataAccess.Dapper;

public class DapperConnectionFactoryTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WithInvalidConnectionString_ThrowsArgumentException(string? connectionString)
    {
        var exception = Assert.Throws<ArgumentException>(() => new DapperConnectionFactory(connectionString!));

        // .NET appends "(Parameter 'connectionString')" to the message.
        Assert.StartsWith("Connection string cannot be null or empty.", exception.Message, StringComparison.Ordinal);
        Assert.Equal("connectionString", exception.ParamName);
    }

    [Fact]
    public void Ctor_WithValidConnectionString_StoresIt()
    {
        var factory = new DapperConnectionFactory("Server=.;Database=Test");

        Assert.Equal("Server=.;Database=Test", factory.ConnectionString);
    }

    [Fact]
    public void CreateConnection_ReturnsSqlConnectionWithConnectionString()
    {
        var factory = new DapperConnectionFactory("Server=.;Database=Test");

        using var connection = factory.CreateConnection();

        var sqlConnection = Assert.IsType<SqlConnection>(connection);
        Assert.Equal("Server=.;Database=Test", sqlConnection.ConnectionString);
        Assert.Equal(ConnectionState.Closed, connection.State);
    }
}

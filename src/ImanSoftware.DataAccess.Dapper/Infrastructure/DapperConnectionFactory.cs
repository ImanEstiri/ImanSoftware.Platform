// مسیر: Infrastructure/DataAccess/Dapper/DapperConnectionFactory.cs
using System.Data;
using Microsoft.Data.SqlClient;

namespace ImanSoftware.DataAccess.Dapper;

/// <summary>
/// Default implementation of IDbConnectionFactory using SqlClient.
/// </summary>
public class DapperConnectionFactory : IDbConnectionFactory
{
    public string ConnectionString { get; }

    public DapperConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

        ConnectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
// Abstractions/DataAccess/IDbConnectionFactory.cs
using System.Data;

namespace ImanSoftware.Framework.Abstractions.DataAccess;

/// <summary>
/// Factory for creating database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new database connection.
    /// </summary>
    IDbConnection CreateConnection();

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    string ConnectionString { get; }
}
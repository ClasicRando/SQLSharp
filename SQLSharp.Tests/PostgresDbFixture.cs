using System.Data.Common;
using JetBrains.Annotations;
using Npgsql;

namespace SQLSharp.Tests;

[UsedImplicitly]
public class PostgresDbFixture : IAsyncLifetime
{
    public DbConnection Connection { get; }
    
    public PostgresDbFixture()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = "127.0.0.1",
            Port = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT")!),
            Username = Environment.GetEnvironmentVariable("POSTGRES_USERNAME"),
            Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"),
            Database = Environment.GetEnvironmentVariable("POSTGRES_DATABASE"),
        };
        Connection = new NpgsqlConnection(builder.ToString());
    }
    
    public Task InitializeAsync()
    {
        return Connection.OpenAsync();
    }

    public Task DisposeAsync()
    {
        return Connection.CloseAsync();
    }
}
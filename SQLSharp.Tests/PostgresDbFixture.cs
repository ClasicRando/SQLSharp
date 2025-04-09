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
    
    public async Task InitializeAsync()
    {
        await Connection.OpenAsync();
        await using DbCommand command = Connection.CreateCommand();
        command.CommandText = """
                              CREATE OR REPLACE PROCEDURE public.mock_procedure(out p_int int)
                              LANGUAGE 'plpgsql'
                              AS $$
                              BEGIN
                                  $1 := 10;
                              END;
                              $$;
                              """;
        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await using DbCommand command = Connection.CreateCommand();
        command.CommandText = "DROP PROCEDURE IF EXISTS public.mock_procedure(out int);";
        await command.ExecuteNonQueryAsync();
        await Connection.CloseAsync();
    }
}
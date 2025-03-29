// See https://aka.ms/new-console-template for more information

using Npgsql;
using SQLSharp.Extensions;
using SQLSharp.Result;

var builder = new NpgsqlConnectionStringBuilder
{
    Host = "127.0.0.1",
    Port = 5432,
    Username = Environment.GetEnvironmentVariable("EXAMPLE_USERNAME"),
    Password = Environment.GetEnvironmentVariable("EXAMPLE_PASSWORD"),
    Database = Environment.GetEnvironmentVariable("EXAMPLE_DATABASE"),
};
await using var connection = new NpgsqlConnection(builder.ToString());

var value = connection.QueryScalar<int>("SELECT 1");

Console.WriteLine($"Value: {value}");

var rows = connection.QueryAsync<Row>(
    """
    SELECT
        gen_random_uuid() as id,
        'Name' as name,
        0 as "age",
        '2025-03-28'::timestamp as date_of_birth
    """);

await foreach (var row in rows)
{
    Console.WriteLine(row.ToString());
}

internal readonly struct Row : IFromRow<Row>
{
    private Guid Id { get; init; }
    
    private string Name { get; init; }
    
    private byte Age { get; init; }
    
    private DateTime DateOfBirth { get; init; }

    public override string ToString()
    {
        return $"Row[Id={Id},Name={Name},Age={Age},DateOfBirth={DateOfBirth}]";
    }

    public static Row FromRow(IDataRow row)
    {
        return new Row
        {
            Id = row.GetFieldNotNull<Guid>("id"),
            Name = row.GetFieldAsClassNotNull<string>("name"),
            Age = row.GetFieldNotNull<byte>("age"),
            DateOfBirth = row.GetFieldNotNull<DateTime>("date_of_birth"),
        };
    }
}
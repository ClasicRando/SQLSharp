using Npgsql;
using SQLSharp.Extensions;
using SQLSharp.Generator.Result;
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

var value = connection.QueryScalarValue<int>("SELECT 1");

Console.WriteLine($"Value: {value}");

var rows = connection.QueryAsync<Row>(
    """
    SELECT
        gen_random_uuid() as id,
        'Name' as name,
        0 as "age",
        '2025-03-28'::timestamp as date_of_birth
    """);

await foreach (Row row in rows)
{
    Console.WriteLine(row.ToString());
}

var generatedRows = connection.QueryAsync<GeneratedRow>(
    """
    SELECT
        gen_random_uuid() as id,
        'Name' as name,
        0 as "age",
        '2025-03-28'::timestamp as date_of_birth
    """);

await foreach (GeneratedRow row in generatedRows)
{
    Console.WriteLine(row.ToString());
}

internal readonly record struct Row : IFromRow<Row>
{
    public Guid Id { get; init; }
    
    public string Name { get; init; }
    
    public byte Age { get; init; }
    
    public DateTime DateOfBirth { get; init; }

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

[FromRow]
internal readonly partial struct InnerRow(
    string name,
    byte age,
    [Column(Name = "date_of_birth")] DateTime? dateOfBirth)
{
    public override string ToString()
    {
        return $"InnerRow[name={name},age={age},dateOfBirth={dateOfBirth}]";
    }
}

[FromRow]
internal readonly partial struct GeneratedRow(
    Guid id,
    [Column(Flatten = true)] InnerRow innerRow)
{
    public override string ToString()
    {
        return $"GeneratedRow[id={id},innerRow={innerRow}]";
    }
}

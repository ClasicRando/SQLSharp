using System.Data;
using Npgsql;
using SQLSharp.Command;
using SQLSharp.Extensions;
using SQLSharp.Generator.Result;
using SQLSharp.Generator.Types;
using SQLSharp.Result;
using SQLSharp.Types;

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

var generatedRows2 = connection.QueryAsync<GeneratedRow2>(
    """
    SELECT
        gen_random_uuid() as "Id",
        2940 as "UniqueId"
    """);

await foreach (GeneratedRow2 row in generatedRows2)
{
    Console.WriteLine(row.ToString());
}

var initRows = connection.QueryAsync<InitRow>(
    """
    SELECT
        gen_random_uuid() as id,
        'Init Name' as name,
        20 as "age",
        null as date_of_birth
    """);

await foreach (InitRow row in initRows)
{
    Console.WriteLine(row.ToString());
}

await connection.ExecuteAsync(
    """
    CREATE OR REPLACE PROCEDURE public.test_proc_ext(in out int, in out text)
    LANGUAGE plpgsql
    AS $$
    BEGIN
        $1 := COALESCE($1,0) + 1;
        $2 := LTRIM($2 || ',' || $1, ',');
    END;
    $$;
    """);
SqlSharpParameters parameters = new();
parameters.Add("IntVar", 1, ParameterDirection.InputOutput);
parameters.Add("TextVar", null, ParameterDirection.InputOutput);
await connection.ExecuteAsync(query: "CALL public.test_proc_ext(@IntVar::int, @TextVar::text)", parameters: parameters);
Console.WriteLine(parameters.Get<int>("IntVar"));
Console.WriteLine(parameters.Get<string?>("TextVar") is null);

internal readonly record struct Row : IFromRow<Row>
{
    public UserId Id { get; init; }
    
    public string Name { get; init; }
    
    public Age Age { get; init; }
    
    public DateTime? DateOfBirth { get; init; }

    public static Row FromRow(IDataRow row)
    {
        return new Row
        {
            Id = UserId.Decode(row, row.IndexOf("id")),
            Name = row.GetFieldNotNull<string>("name"),
            Age = Age.Decode(row, row.IndexOf("age")),
            DateOfBirth = row.GetField<DateTime?>("date_of_birth"),
        };
    }
}

[FromRow(RenameAll = Rename.SnakeCase)]
internal readonly partial struct InnerRow(string name, byte age, DateTime? dateOfBirth)
{
    public override string ToString()
    {
        return $"InnerRow[name={name},age={age},dateOfBirth={dateOfBirth}]";
    }
}

[FromRow]
internal readonly partial struct GeneratedRow(Guid id, [Column(Flatten = true)] InnerRow innerRow)
{
    public override string ToString()
    {
        return $"GeneratedRow[id={id},innerRow={innerRow}]";
    }
}

namespace Examples.Test
{
    internal readonly record struct IntValue(int Inner) : IDbDecode<IntValue>
    {
        public static IntValue Decode(IDataRow row, int column)
        {
            return new IntValue(row.GetFieldNotNull<int>(column));
        }
    }
}

[FromRow(RenameAll = Rename.PascalCase)]
internal readonly partial struct GeneratedRow2(Guid id, Examples.Test.IntValue uniqueId)
{
    public override string ToString()
    {
        return $"GeneratedRow[id={id},uniqueId={uniqueId}]";
    }
}

[FromRow(RenameAll = Rename.SnakeCase)]
internal partial class InitRow
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required byte Age { get; init; }
    public required DateTime? DateOfBirth { get; init; }
    
    public override string ToString()
    {
        return $"InitRow[id={Id},Name={Name},Age={Age},DateOfBirth={DateOfBirth}]";
    }
}

[WrapperType]
internal partial struct UserId(Guid inner)
{
    private readonly Guid _inner = inner;

    public override string ToString()
    {
        return $"UserId({_inner})";
    }
}

[WrapperType]
internal partial class Age(byte inner)
{
    public byte Inner { get; } = inner;

    public override string ToString()
    {
        return $"Age({Inner})";
    }
}

namespace SQLSharp.Generator.Tests;

[UsesVerify]
public class FromRowGeneratorSnapshotTests
{
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_StructTypeWithConstructor()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow]
                              internal readonly partial struct GeneratedRow(Guid id, string name, byte age, DateTime? dateOfBirth);
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_StructTypeWithConstructorAndColumnAttribute()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow]
                              internal readonly partial struct GeneratedRow(
                                  Guid id,
                                  string name,
                                  byte age,
                                  [Column(Name = "date_of_birth")] DateTime? dateOfBirth);
                              """;

        return TestHelper.Verify(source);
    }
}
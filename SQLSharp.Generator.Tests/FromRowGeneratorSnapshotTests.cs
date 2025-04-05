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
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_StructTypeWithConstructorAndRenameAllSnakeCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow(RenameAll = Rename.SnakeCase)]
                              internal readonly partial struct GeneratedRow(
                                  Guid id,
                                  string name,
                                  byte age,
                                  DateTime? dateOfBirth);
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_StructTypeWithConstructorAndRenameAllCamelCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow(RenameAll = Rename.CamelCase)]
                              internal partial record GeneratedRow(
                                  Guid Id,
                                  string Name,
                                  byte Age,
                                  DateTime? DateOfBirth);
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_StructTypeWithConstructorAndRenameAllPascalCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow(RenameAll = Rename.PascalCase)]
                              internal readonly partial struct GeneratedRow(
                                  Guid id,
                                  string name,
                                  byte age,
                                  DateTime? dateOfBirth);
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_StructTypeWithConstructorAndRenameAllUpperCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow(RenameAll = Rename.UpperCase)]
                              internal readonly partial struct GeneratedRow(
                                  Guid id,
                                  string name,
                                  byte age,
                                  DateTime? dateOfBirth);
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_StructTypeWithConstructorAndRenameAllLowerCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow(RenameAll = Rename.LowerCase)]
                              internal readonly partial struct GeneratedRow(
                                  Guid id,
                                  string name,
                                  byte age,
                                  DateTime? dateOfBirth);
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_StructTypeWithConstructorAndRenameAllNone()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow(RenameAll = Rename.None)]
                              internal readonly partial struct GeneratedRow(
                                  Guid id,
                                  string NaMe,
                                  byte aGe,
                                  DateTime? daTeOFBiRth);
                              """;

        return TestHelper.Verify(source);
    }
}
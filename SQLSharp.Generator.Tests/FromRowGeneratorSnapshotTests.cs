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
                                  [Column(Rename = "date_of_birth")] DateTime? dateOfBirth);
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
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationRespectingColumnAttribute_when_StructTypeWithConstructorAndRenameAllSnakeCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;

                              [FromRow(RenameAll = Rename.SnakeCase)]
                              internal readonly partial struct GeneratedRow(
                                  Guid id,
                                  string name,
                                  byte age,
                                  [Column(Rename = "DateOfBirth")] DateTime? dateOfBirth);
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_ClassTypeWithPropertyInitializers()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;

                              [FromRow]
                              internal partial class GeneratedRow
                              {
                                  public Guid Id { get; init; }
                                  public string Name { get; init; }
                                  public byte Age { get; init; }
                                  public DateTime? DateOfBirth { get; init; }
                                  public int ConstValue { get; } = 10;
                              }
                              """;

        return TestHelper.Verify(source);
    }
    
    
    
    
    
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_ClassTypeWithPropertyInitializersAndColumnAttribute()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;
                              
                              [FromRow]
                              internal partial class GeneratedRow
                              {
                                  public Guid Id { get; init; }
                                  public string Name { get; init; }
                                  public byte Age { get; init; }
                                  public DateTime? DateOfBirth { get; init; }
                              }
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_ClassTypeWithPropertyInitializersAndRenameAllSnakeCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;
                              
                              [FromRow(RenameAll = Rename.SnakeCase)]
                              internal partial class GeneratedRow
                              {
                                  public Guid Id { get; init; }
                                  public string Name { get; init; }
                                  public byte Age { get; init; }
                                  public DateTime? DateOfBirth { get; init; }
                              }
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_ClassTypeWithPropertyInitializersAndRenameAllCamelCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;
                              
                              [FromRow(RenameAll = Rename.CamelCase)]
                              internal partial class GeneratedRow
                              {
                                  public Guid Id { get; init; }
                                  public string Name { get; init; }
                                  public byte Age { get; init; }
                                  public DateTime? DateOfBirth { get; init; }
                              }
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_ClassTypeWithPropertyInitializersAndRenameAllPascalCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;
                              
                              [FromRow(RenameAll = Rename.PascalCase)]
                              internal partial class GeneratedRow
                              {
                                  public Guid id { get; init; }
                                  public string name { get; init; }
                                  public byte age { get; init; }
                                  public DateTime? dateOfBirth { get; init; }
                              }
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_ClassTypeWithPropertyInitializersAndRenameAllUpperCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;
                              
                              [FromRow(RenameAll = Rename.UpperCase)]
                              internal partial class GeneratedRow
                              {
                                  public Guid Id { get; init; }
                                  public string Name { get; init; }
                                  public byte Age { get; init; }
                                  public DateTime? DateOfBirth { get; init; }
                              }
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_ClassTypeWithPropertyInitializersAndRenameAllLowerCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;
                              
                              [FromRow(RenameAll = Rename.LowerCase)]
                              internal partial class GeneratedRow
                              {
                                  public Guid Id { get; init; }
                                  public string Name { get; init; }
                                  public byte Age { get; init; }
                                  public DateTime? DateOfBirth { get; init; }
                              }
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationCorrectly_when_ClassTypeWithPropertyInitializersAndRenameAllNone()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;
                              
                              [FromRow(RenameAll = Rename.None)]
                              internal partial class GeneratedRow
                              {
                                  public Guid id { get; init; }
                                  public string NaMe { get; init; }
                                  public byte aGe { get; init; }
                                  public DateTime? daTeOFBiRth { get; init; }
                              }
                              """;

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task Should_GeneratesIFromRowImplementationRespectingColumnAttribute_when_ClassTypeWithPropertyInitializersAndRenameAllSnakeCase()
    {
        const string source = """
                              using SQLSharp.Generator.Result;
                              using System;
                              
                              [FromRow(RenameAll = Rename.SnakeCase)]
                              internal partial class GeneratedRow
                              {
                                  public Guid Id { get; init; }
                                  public string Name { get; init; }
                                  public byte Age { get; init; }
                                  [Column(Rename = "DateOfBirth")]
                                  public DateTime? DateOfBirth { get; init; }
                              }
                              """;

        return TestHelper.Verify(source);
    }
}
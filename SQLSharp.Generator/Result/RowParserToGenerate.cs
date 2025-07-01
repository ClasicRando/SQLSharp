using System.Collections.Immutable;

namespace SQLSharp.Generator.Result;

public record RowParserToGenerate(
    string Name,
    string Namespace,
    bool IsPartial,
    bool IsStruct,
    Rename Rename,
    ImmutableArray<ConstructorData> Constructors,
    InitializerData InitializerData)
{
    public string Name { get; } = Name;
    public string Namespace { get; } = Namespace;
    public bool IsPartial { get; } = IsPartial;
    public bool IsStruct { get; } = IsStruct;
    public Rename Rename { get; } = Rename;
    public ImmutableArray<ConstructorData> Constructors { get; } = Constructors;
    public InitializerData InitializerData { get; } = InitializerData;
}

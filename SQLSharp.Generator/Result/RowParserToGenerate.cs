using System.Collections.Immutable;

namespace SQLSharp.Generator.Result;

internal record RowParserToGenerate(
    string Name,
    string Namespace,
    bool IsPartial,
    bool IsStruct,
    Rename Rename,
    ImmutableArray<ConstructorData> Constructors,
    InitializerData InitializerData)
{
    internal string Name { get; } = Name;
    internal string Namespace { get; } = Namespace;
    internal bool IsPartial { get; } = IsPartial;
    internal bool IsStruct { get; } = IsStruct;
    internal Rename Rename { get; } = Rename;
    internal ImmutableArray<ConstructorData> Constructors { get; } = Constructors;
    internal InitializerData InitializerData { get; } = InitializerData;
}

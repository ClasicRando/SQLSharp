using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Result;

public record RowParserToGenerate
{
    public string Name { get; }
    public string Namespace { get; }
    public bool IsPartial { get; }
    public bool IsStruct { get; }
    public Rename Rename { get; }
    public ImmutableArray<ConstructorData> Constructors { get; }
    public ImmutableArray<ISymbol> Members { get; }

    public RowParserToGenerate(
        string name,
        string typeNamespace,
        bool isPartial,
        bool isStruct,
        Rename rename,
        ImmutableArray<ConstructorData> constructors,
        ImmutableArray<ISymbol> members)
    {
        Name = name;
        Namespace = typeNamespace;
        IsPartial = isPartial;
        IsStruct = isStruct;
        Rename = rename;
        Constructors = constructors;
        Members = members;
    }
}

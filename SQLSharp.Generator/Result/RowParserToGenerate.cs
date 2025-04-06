using System.Collections.Immutable;

namespace SQLSharp.Generator.Result;

public record RowParserToGenerate
{
    public string Name { get; }
    public string Namespace { get; }
    public bool IsPartial { get; }
    public bool IsStruct { get; }
    public Rename Rename { get; }
    public ImmutableArray<ConstructorData> Constructors { get; }
    public InitializerData InitializerData { get; }

    public RowParserToGenerate(
        string name,
        string typeNamespace,
        bool isPartial,
        bool isStruct,
        Rename rename,
        ImmutableArray<ConstructorData> constructors,
        InitializerData initializerData)
    {
        Name = name;
        Namespace = typeNamespace;
        IsPartial = isPartial;
        IsStruct = isStruct;
        Rename = rename;
        Constructors = constructors;
        InitializerData = initializerData;
    }
}

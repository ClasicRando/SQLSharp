namespace SQLSharp.Generator.Types;

public record WrapperTypeToGenerate(
    string Name,
    string Namespace,
    bool IsPartial,
    bool HasNonDefaultConstructor,
    bool IsStruct,
    InnerValueData? InnerValue)
{
    public string Name { get; } = Name;
    public string Namespace { get; } = Namespace;
    public bool IsPartial { get; } = IsPartial;
    public bool HasNonDefaultConstructor { get; } = HasNonDefaultConstructor;
    public bool IsStruct { get; } = IsStruct;
    public InnerValueData? InnerValue { get; } = InnerValue;
}

public record InnerValueData(string Name, string TypeName)
{
    public string Name { get; } = Name;
    public string TypeName { get; } = TypeName;
}

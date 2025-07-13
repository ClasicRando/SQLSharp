namespace SQLSharp.Generator.Types;

internal record WrapperTypeToGenerate(
    string Name,
    string Namespace,
    bool IsPartial,
    bool HasNonDefaultConstructor,
    bool IsStruct,
    InnerValueData? InnerValue)
{
    internal string Name { get; } = Name;
    internal string Namespace { get; } = Namespace;
    internal bool IsPartial { get; } = IsPartial;
    internal bool HasNonDefaultConstructor { get; } = HasNonDefaultConstructor;
    internal bool IsStruct { get; } = IsStruct;
    internal InnerValueData? InnerValue { get; } = InnerValue;
}

internal record InnerValueData(string Name, string TypeName, bool IsRefType, bool IsNullable)
{
    internal string Name { get; } = Name;
    internal string TypeName { get; } = TypeName;
    internal bool IsRefType { get; } = IsRefType;
    internal bool IsNullable { get; } = IsNullable;
}

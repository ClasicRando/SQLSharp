using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Result;

public record ConstructorData
{
    public ImmutableArray<ParameterData> Parameters { get; }

    private ConstructorData(ImmutableArray<ParameterData> parameters)
    {
        Parameters = parameters;
    }

    public static ConstructorData FromMethodSymbol(
        IMethodSymbol methodSymbol,
        INamedTypeSymbol columnAttribute)
    {
        var parameters = methodSymbol.Parameters
            .Select(p => ParameterData.FromParameterSymbol(p, columnAttribute))
            .ToImmutableArray();
        return new ConstructorData(parameters);
    }
}

public record ParameterData
{
    public string Name { get; }
    public ParameterTypeData TypeData { get; }

    private ParameterData(string name, ParameterTypeData typeData)
    {
        Name = name;
        TypeData = typeData;
    }
    
    public static ParameterData FromParameterSymbol(
        IParameterSymbol parameterSymbol,
        INamedTypeSymbol columnAttribute)
    {
        var name = parameterSymbol
            .GetAttributes()
            .Where(a => columnAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default))
            .Select(a =>
            {
                return a.NamedArguments
                    .Where(na => na.Key == "Name")
                    .Select(na => na.Value.Value?.ToString())
                    .FirstOrDefault();
            })
            .FirstOrDefault(name => name is not null) ?? parameterSymbol.Name;
        var isNullable = parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated;
        var typeName = isNullable
            ? ((INamedTypeSymbol)parameterSymbol.Type).TypeArguments.First().Name
            : parameterSymbol.Type.Name;
        var typeData = new ParameterTypeData(
            typeName,
            parameterSymbol.Type.ContainingNamespace.Name,
            parameterSymbol.Type.TypeKind is TypeKind.Array or TypeKind.Class,
            parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated);
        return new ParameterData(name, typeData);
    }
}

public record ParameterTypeData
{
    public string Name { get; }
    public string ContainingNamespace { get; }
    public bool IsRefType { get; }
    public bool IsNullable { get; }

    public ParameterTypeData(
        string name,
        string containingNamespace,
        bool isRefType,
        bool isNullable)
    {
        Name = name;
        ContainingNamespace = containingNamespace;
        IsRefType = isRefType;
        IsNullable = isNullable;
    }
}
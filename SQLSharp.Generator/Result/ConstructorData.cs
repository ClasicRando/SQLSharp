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
    public string ResultFieldName { get; }
    public bool Flatten { get; }
    public ParameterTypeData TypeData { get; }

    private ParameterData(
        string name,
        string resultFieldName,
        bool flatten,
        ParameterTypeData typeData)
    {
        Name = name;
        ResultFieldName = resultFieldName;
        Flatten = flatten;
        TypeData = typeData;
    }
    
    public static ParameterData FromParameterSymbol(
        IParameterSymbol parameterSymbol,
        INamedTypeSymbol columnAttribute)
    {
        AttributeData? attributeData = parameterSymbol
            .GetAttributes()
            .FirstOrDefault(a => columnAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
        var resultFieldName = parameterSymbol.Name;
        var flatten = false;
        if (attributeData is not null)
        {
            foreach (var kvp in attributeData.NamedArguments)
            {
                var value = kvp.Value.Value;
                switch (kvp.Key)
                {
                    case "Name":
                    {
                        var attributeName = value?.ToString();
                        if (string.IsNullOrEmpty(attributeName))
                        {
                            continue;
                        }
                        resultFieldName = attributeName!;
                        break;
                    }
                    case "Flatten":
                    {
                        if (value is bool b)
                        {
                            flatten = b;
                        }
                        break;
                    }
                }
            }
        }
        var isNullable = parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated;
        var typeName = isNullable
            ? ((INamedTypeSymbol)parameterSymbol.Type).TypeArguments.First().Name
            : parameterSymbol.Type.Name;
        var typeData = new ParameterTypeData(
            typeName,
            parameterSymbol.Type.ContainingNamespace.Name,
            parameterSymbol.Type.TypeKind is TypeKind.Array or TypeKind.Class,
            parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated);
        return new ParameterData(parameterSymbol.Name, resultFieldName, flatten, typeData);
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
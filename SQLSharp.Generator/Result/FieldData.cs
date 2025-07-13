using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Result;

internal record FieldData(
    string Name,
    string ResultFieldName,
    bool HasRename,
    bool Flatten,
    FieldTypeData TypeData)
{
    internal string Name { get; } = Name;
    internal string ResultFieldName { get; } = ResultFieldName;
    internal bool HasRename { get; } = HasRename;
    internal bool Flatten { get; } = Flatten;
    internal FieldTypeData TypeData { get; } = TypeData;
    
    internal static FieldData FromPropertySymbol(
        IPropertySymbol symbol,
        INamedTypeSymbol columnAttribute)
    {
        AttributeData? attributeData = symbol
            .GetAttributes()
            .FirstOrDefault(a => columnAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
        var resultFieldName = symbol.Name;
        var hasRename = false;
        var flatten = false;
        if (attributeData is not null)
        {
            foreach (var kvp in attributeData.NamedArguments)
            {
                var value = kvp.Value.Value;
                switch (kvp.Key)
                {
                    case "Rename":
                    {
                        var attributeValue = value?.ToString();
                        if (string.IsNullOrEmpty(attributeValue))
                        {
                            continue;
                        }

                        hasRename = true;
                        resultFieldName = attributeValue!;
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
        
        var isNullable = symbol.NullableAnnotation == NullableAnnotation.Annotated;
        var isRefType = symbol.Type.TypeKind is TypeKind.Array or TypeKind.Class;
        var typeName = isNullable && !isRefType
            ? ((INamedTypeSymbol)symbol.Type).TypeArguments.First().Name
            : symbol.Type.Name;
        var typeData = new FieldTypeData(
            typeName,
            symbol.Type.ContainingNamespace.GetFullNamespaceName(),
            isRefType,
            isNullable,
            symbol.Type.AllInterfaces.Any(t => t.Name == "IDbDecode"));
        return new FieldData(
            symbol.Name,
            resultFieldName,
            hasRename,
            flatten,
            typeData);
    }
    
    internal static FieldData FromParameterSymbol(
        IParameterSymbol parameterSymbol,
        INamedTypeSymbol columnAttribute)
    {
        AttributeData? attributeData = parameterSymbol
            .GetAttributes()
            .FirstOrDefault(a => columnAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
        var resultFieldName = parameterSymbol.Name;
        var hasRename = false;
        var flatten = false;
        if (attributeData is not null)
        {
            foreach (var kvp in attributeData.NamedArguments)
            {
                var value = kvp.Value.Value;
                switch (kvp.Key)
                {
                    case "Rename":
                    {
                        var attributeValue = value?.ToString();
                        if (string.IsNullOrEmpty(attributeValue))
                        {
                            continue;
                        }

                        hasRename = true;
                        resultFieldName = attributeValue!;
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
        var isRefType = parameterSymbol.Type.TypeKind is TypeKind.Array or TypeKind.Class;
        var typeName = isNullable && !isRefType
            ? ((INamedTypeSymbol)parameterSymbol.Type).TypeArguments.First().Name
            : parameterSymbol.Type.Name;
        var typeData = new FieldTypeData(
            typeName,
            parameterSymbol.Type.ContainingNamespace.GetFullNamespaceName(),
            isRefType,
            isNullable,
            parameterSymbol.Type.AllInterfaces.Any(t => t.Name == "IDbDecode"));
        return new FieldData(
            parameterSymbol.Name,
            resultFieldName,
            hasRename,
            flatten,
            typeData);
    }
}

internal record FieldTypeData(
    string Name,
    string ContainingNamespace,
    bool IsRefType,
    bool IsNullable,
    bool IsDecode)
{
    internal string Name { get; } = Name;
    internal string ContainingNamespace { get; } = ContainingNamespace;
    internal bool IsRefType { get; } = IsRefType;
    internal bool IsNullable { get; } = IsNullable;
    internal bool IsDecode { get; } = IsDecode;
}

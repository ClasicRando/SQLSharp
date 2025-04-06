using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Result;

public record FieldData
{
    public string Name { get; }
    public string ResultFieldName { get; }
    public bool HasRename { get; }
    public bool Flatten { get; }
    public FieldTypeData TypeData { get; }

    private FieldData(
        string name,
        string resultFieldName,
        bool hasRename,
        bool flatten,
        FieldTypeData typeData)
    {
        Name = name;
        ResultFieldName = resultFieldName;
        HasRename = hasRename;
        Flatten = flatten;
        TypeData = typeData;
    }
    
    public static FieldData FromPropertySymbol(
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
        var typeName = isNullable
            ? ((INamedTypeSymbol)symbol.Type).TypeArguments.First().Name
            : symbol.Type.Name;
        var typeData = new FieldTypeData(
            typeName,
            symbol.Type.ContainingNamespace.GetFullNamespaceName(),
            symbol.Type.TypeKind is TypeKind.Array or TypeKind.Class,
            symbol.NullableAnnotation == NullableAnnotation.Annotated,
            symbol.Type.AllInterfaces.Any(t => t.Name == "IDbDecode"));
        return new FieldData(
            symbol.Name,
            resultFieldName,
            hasRename,
            flatten,
            typeData);
    }
    
    public static FieldData FromParameterSymbol(
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
        var typeName = isNullable
            ? ((INamedTypeSymbol)parameterSymbol.Type).TypeArguments.First().Name
            : parameterSymbol.Type.Name;
        var typeData = new FieldTypeData(
            typeName,
            parameterSymbol.Type.ContainingNamespace.GetFullNamespaceName(),
            parameterSymbol.Type.TypeKind is TypeKind.Array or TypeKind.Class,
            parameterSymbol.NullableAnnotation == NullableAnnotation.Annotated,
            parameterSymbol.Type.AllInterfaces.Any(t => t.Name == "IDbDecode"));
        return new FieldData(
            parameterSymbol.Name,
            resultFieldName,
            hasRename,
            flatten,
            typeData);
    }
}

public record FieldTypeData
{
    public string Name { get; }
    public string ContainingNamespace { get; }
    public bool IsRefType { get; }
    public bool IsNullable { get; }
    public bool IsDecode { get; }

    public FieldTypeData(
        string name,
        string containingNamespace,
        bool isRefType,
        bool isNullable,
        bool isDecode)
    {
        Name = name;
        ContainingNamespace = containingNamespace;
        IsRefType = isRefType;
        IsNullable = isNullable;
        IsDecode = isDecode;
    }
}

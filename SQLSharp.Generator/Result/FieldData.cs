using Microsoft.CodeAnalysis;
using SQLSharp.Generator.Common;

namespace SQLSharp.Generator.Result;

public record FieldData
{
    public string Name { get; }
    public string ResultFieldName { get; }
    public bool HasRename { get; }
    public bool Flatten { get; }
    public TypeData TypeData { get; }

    private FieldData(
        string name,
        string resultFieldName,
        bool hasRename,
        bool flatten,
        TypeData typeData)
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
        return new FieldData(
            symbol.Name,
            resultFieldName,
            hasRename,
            flatten,
            TypeData.FromTypeSymbol(symbol.Type, isNullable));
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
        return new FieldData(
            parameterSymbol.Name,
            resultFieldName,
            hasRename,
            flatten,
            TypeData.FromTypeSymbol(parameterSymbol.Type, isNullable));
    }
}

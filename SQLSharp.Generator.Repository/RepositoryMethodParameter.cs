using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Repository;

public record RepositoryMethodParameter
{
    public string Name { get; }
    public string QueryParameterName { get; }

    private RepositoryMethodParameter(string name, string queryParameterName)
    {
        Name = name;
        QueryParameterName = queryParameterName;
    }

    public static RepositoryMethodParameter FromParameterSymbol(
        IParameterSymbol parameterSymbol,
        INamedTypeSymbol parameterAttribute)
    {
        var queryParameterName = parameterSymbol.GetAttributes()
            .FirstOrDefault(a =>
                parameterAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default))
            ?.NamedArguments
            .FirstOrDefault(na => na.Key == "Rename")
            .Value
            .Value
            ?.ToString() ?? parameterSymbol.Name;
        return new RepositoryMethodParameter(parameterSymbol.Name, queryParameterName);
    }
}

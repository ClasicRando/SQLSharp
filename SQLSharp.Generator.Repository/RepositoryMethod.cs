using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SQLSharp.Generator.Common;

namespace SQLSharp.Generator.Repository;

public record RepositoryMethod
{
    public string Name { get; }
    public string Query { get; }
    public TypeData ReturnType { get; }
    public ImmutableArray<RepositoryMethodParameter> Parameters { get; }

    private RepositoryMethod(
        string name,
        string query,
        TypeData returnType,
        ImmutableArray<RepositoryMethodParameter> parameters)
    {
        Name = name;
        Query = query;
        ReturnType = returnType;
        Parameters = parameters;
    }

    public static RepositoryMethod? FromMethodSymbol(
        IMethodSymbol methodSymbol,
        INamedTypeSymbol queryAttribute,
        INamedTypeSymbol parameterAttribute)
    {
        AttributeData? queryAttr = methodSymbol.GetAttributes()
            .FirstOrDefault(a =>
                queryAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
        if (queryAttr == null)
        {
            return null;
        }
        var query = queryAttr.NamedArguments
            .First(na => na.Key == "Query")
            .Value
            .ToString();
        var isNullable = methodSymbol.ReturnNullableAnnotation == NullableAnnotation.Annotated;
        return new RepositoryMethod(
            methodSymbol.Name,
            query,
            TypeData.FromTypeSymbol(methodSymbol.ReturnType, isNullable),
            methodSymbol.Parameters
                .Select(p => RepositoryMethodParameter.FromParameterSymbol(p, parameterAttribute))
                .ToImmutableArray());
    }
}

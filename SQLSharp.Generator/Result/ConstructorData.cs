using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Result;

public record ConstructorData(ImmutableArray<FieldData> Parameters)
{
    public ImmutableArray<FieldData> Parameters { get; } = Parameters;

    public static ConstructorData FromMethodSymbol(
        IMethodSymbol methodSymbol,
        INamedTypeSymbol columnAttribute)
    {
        var parameters = methodSymbol.Parameters
            .Select(p => FieldData.FromParameterSymbol(p, columnAttribute))
            .ToImmutableArray();
        return new ConstructorData(parameters);
    }
}

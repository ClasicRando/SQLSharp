using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Result;

internal record ConstructorData(ImmutableArray<FieldData> Parameters)
{
    internal ImmutableArray<FieldData> Parameters { get; } = Parameters;

    internal static ConstructorData FromMethodSymbol(
        IMethodSymbol methodSymbol,
        INamedTypeSymbol columnAttribute)
    {
        var parameters = methodSymbol.Parameters
            .Select(p => FieldData.FromParameterSymbol(p, columnAttribute))
            .ToImmutableArray();
        return new ConstructorData(parameters);
    }
}

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Result;

internal class InitializerData(ImmutableArray<FieldData> properties)
{
    internal ImmutableArray<FieldData> Properties { get; } = properties;

    internal static InitializerData FromTypeSymbol(
        ITypeSymbol typeSymbol,
        INamedTypeSymbol columnAttribute)
    {
        var parameters = typeSymbol.GetMembers()
            .Select(s => s as IPropertySymbol)
            .Where(p => p is not null && !p.IsReadOnly)
            .Select(p => FieldData.FromPropertySymbol(p!, columnAttribute))
            .ToImmutableArray();
        return new InitializerData(parameters);
    }
}

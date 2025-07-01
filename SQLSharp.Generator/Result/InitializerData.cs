using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Result;

public class InitializerData(ImmutableArray<FieldData> properties)
{
    public ImmutableArray<FieldData> Properties { get; } = properties;

    public static InitializerData FromTypeSymbol(
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

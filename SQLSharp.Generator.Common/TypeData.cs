using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Common;

public record TypeData
{
    public string Name { get; }
    public string ContainingNamespace { get; }
    public bool IsRefType { get; }
    public bool IsNullable { get; }
    public bool IsDecode { get; }

    private TypeData(
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

    public static TypeData FromTypeSymbol(ITypeSymbol typeSymbol, bool isNullable)
    {
        var typeName = isNullable
            ? ((INamedTypeSymbol)typeSymbol).TypeArguments.First().Name
            : typeSymbol.Name;
        return new TypeData(
            typeName,
            typeSymbol.ContainingNamespace.GetFullNamespaceName(),
            typeSymbol.TypeKind is TypeKind.Array or TypeKind.Class,
            isNullable,
            typeSymbol.AllInterfaces.Any(t => t.Name == "IDbDecode"));
    }
}

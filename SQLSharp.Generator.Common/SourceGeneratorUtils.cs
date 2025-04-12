using System.Text;
using Microsoft.CodeAnalysis;

namespace SQLSharp.Generator.Common;

public static class SourceGeneratorUtils
{
    public static void AppendFullTypeName(
        this StringBuilder builder,
        TypeData typeData)
    {
        if (!string.IsNullOrEmpty(typeData.ContainingNamespace))
        {
            builder.Append(typeData.ContainingNamespace);
            builder.Append('.');
        }
        builder.Append(typeData.Name);
    }
    
    public static string GetFullNamespaceName(this INamespaceSymbol namespaceSymbol)
    {
        if (string.IsNullOrEmpty(namespaceSymbol.Name))
        {
            return string.Empty;
        }
        
        StringBuilder builder = new(namespaceSymbol.Name);
        INamespaceSymbol currentNamespace = namespaceSymbol.ContainingNamespace;
        while (!string.IsNullOrEmpty(currentNamespace.Name))
        {
            builder.Insert(0, '.');
            builder.Insert(0, currentNamespace.Name);
            currentNamespace = currentNamespace.ContainingNamespace;
        }
        return builder.ToString();
    }
}

using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SQLSharp.Generator.Types;

[Generator]
public class WrapperTypeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource(
                "WrapperTypeAttribute.g.cs",
                SourceText.From(SourceGenerationHelper.WrapperTypeAttribute, Encoding.UTF8));
        });

        var typesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "SQLSharp.Generator.Types.WrapperTypeAttribute",
                predicate: static (_, _) => true,
                transform: static (ctx, _) =>
                    GetTargetForGeneration(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(typesToGenerate.Collect(),
            static (spc, source) => Execute(spc, source));
    }

    private static INamedTypeSymbol? GetTargetForGeneration(
        SemanticModel semanticModel,
        SyntaxNode syntax)
    {
        return semanticModel.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
    }

    private static void Execute(
        SourceProductionContext context,
        ImmutableArray<INamedTypeSymbol?> typesToGenerate)
    {
        if (typesToGenerate.IsDefaultOrEmpty) return;

        foreach (INamedTypeSymbol? typeSymbol in typesToGenerate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (typeSymbol is null)
            {
                continue;
            }
            
            WrapperTypeToGenerate wrapperTypeToGenerate = GetWrapperTypeToGenerate(typeSymbol);
            var sourceCode = SourceGenerationHelper.GenerateWrapperDecode(wrapperTypeToGenerate, context);
            context.AddSource($"WrapperTypes.{wrapperTypeToGenerate.Name}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }
    
    private static WrapperTypeToGenerate GetWrapperTypeToGenerate(INamedTypeSymbol typeSymbol)
    {
        var name = typeSymbol.Name;
        var typeNamespace = typeSymbol.ContainingNamespace.GetFullNamespaceName();
        var isPartial = typeSymbol.DeclaringSyntaxReferences.Any(s =>
            s.GetSyntax() is BaseTypeDeclarationSyntax declaration &&
            declaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));
        var hasNonDefaultConstructor = typeSymbol.Constructors.Any(c => c.Parameters.Length > 0);
        var isStruct = typeSymbol.IsValueType;
        
        var members = typeSymbol.GetMembers();
        var properties = members.OfType<IPropertySymbol>()
            .Where(p => !p.IsImplicitlyDeclared)
            .ToImmutableArray();
        var fields = members.OfType<IFieldSymbol>()
            .Where(f => !f.IsImplicitlyDeclared)
            .ToImmutableArray();

        if (fields.Length != 1 && properties.Length != 1)
        {
            return new WrapperTypeToGenerate(
                name,
                typeNamespace,
                isPartial,
                hasNonDefaultConstructor,
                isStruct,
                null);
        }

        string valueName;
        ITypeSymbol innerType;
        if (fields.Length == 1)
        {
            valueName = fields[0].Name;
            innerType = fields[0].Type;
        }
        else
        {
            valueName = properties[0].Name;
            innerType = properties[0].Type;
        }

        string innerTypeSimpleName;
        string innerTypeNamespace;
        if (innerType is IArrayTypeSymbol arrayTypeSymbol)
        {
            innerTypeNamespace = arrayTypeSymbol.ElementType.ContainingNamespace.GetFullNamespaceName();
            innerTypeSimpleName = $"{arrayTypeSymbol.ElementType.Name}[]";
        }
        else
        {
            innerTypeNamespace = innerType.ContainingNamespace.GetFullNamespaceName();
            innerTypeSimpleName = innerType.Name;
        }

        var innerTypeName = string.IsNullOrEmpty(innerTypeNamespace)
            ? innerTypeSimpleName
            : $"{innerTypeNamespace}.{innerTypeSimpleName}";
        
        return new WrapperTypeToGenerate(
            name,
            typeNamespace,
            isPartial,
            hasNonDefaultConstructor,
            isStruct,
            InnerValue: new InnerValueData(valueName, innerTypeName));
    }
}

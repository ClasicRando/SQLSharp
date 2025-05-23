using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SQLSharp.Generator.Result;

[Generator]
public class FromRowGenerator : IIncrementalGenerator
{
    private const string ColumnAttributeName = "SQLSharp.Generator.Result.ColumnAttribute";
    private const string FromRowAttributeName = "SQLSharp.Generator.Result.FromRowAttribute";
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource(
                "Rename.g.cs",
                SourceText.From(SourceGenerationHelper.RenameEnum, Encoding.UTF8));
            ctx.AddSource(
                "FromRowAttribute.g.cs",
                SourceText.From(SourceGenerationHelper.FromRowAttribute, Encoding.UTF8));
            ctx.AddSource(
                "ColumnAttribute.g.cs",
                SourceText.From(SourceGenerationHelper.ColumnAttribute, Encoding.UTF8));
        });

        var typesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "SQLSharp.Generator.Result.FromRowAttribute",
                predicate: static (_, _) => true,
                transform: static (ctx, _) =>
                    GetTargetForGeneration(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        var compilationAndTypes = context.CompilationProvider.Combine(typesToGenerate.Collect());

        context.RegisterSourceOutput(compilationAndTypes,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static INamedTypeSymbol? GetTargetForGeneration(
        SemanticModel semanticModel,
        SyntaxNode syntax)
    {
        return semanticModel.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<INamedTypeSymbol?> typesToGenerate,
        SourceProductionContext context)
    {
        if (typesToGenerate.IsDefaultOrEmpty) return;

        foreach (INamedTypeSymbol? typeSymbol in typesToGenerate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (typeSymbol is null)
            {
                continue;
            }
            
            RowParserToGenerate? rowParserToGenerate = GetRowParserToGenerate(compilation, typeSymbol);
            if (rowParserToGenerate is null)
            {
                continue;
            }
            
            var sourceCode = SourceGenerationHelper.GenerateRowParserPartialClass(rowParserToGenerate, context);
            context.AddSource($"RowParsers.{rowParserToGenerate.Name}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }
    
    private static RowParserToGenerate? GetRowParserToGenerate(
        Compilation compilation,
        INamedTypeSymbol typeSymbol)
    {
        INamedTypeSymbol? columnAttribute = compilation.GetTypeByMetadataName(ColumnAttributeName);
        if (columnAttribute is null)
        {
            return null;
        }

        INamedTypeSymbol? fromRowAttribute = compilation.GetTypeByMetadataName(FromRowAttributeName);
        if (fromRowAttribute is null)
        {
            return null;
        }

        Rename rename = typeSymbol.GetAttributes()
            .FirstOrDefault(a =>
                fromRowAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default))
            ?.NamedArguments
            .Where(na => na.Key == "RenameAll")
            .Select(na => na.Value.Value)
            .FirstOrDefault()
            ?.ParseToRename() ?? Rename.None;
        
        return new RowParserToGenerate(
            typeSymbol.Name,
            typeSymbol.ContainingNamespace.Name,
            typeSymbol.DeclaringSyntaxReferences.Any(s =>
                s.GetSyntax() is BaseTypeDeclarationSyntax declaration &&
                declaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))),
            typeSymbol.IsValueType,
            rename,
            typeSymbol.Constructors
                .Where(c => c.Parameters.Length > 0)
                .Select(c => ConstructorData.FromMethodSymbol(c, columnAttribute))
                .ToImmutableArray(),
            InitializerData.FromTypeSymbol(typeSymbol, columnAttribute));
    }
}

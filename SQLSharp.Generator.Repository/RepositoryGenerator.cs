using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SQLSharp.Generator.Common;

namespace SQLSharp.Generator.Repository;

[Generator]
public class RepositoryGenerator : IIncrementalGenerator
{
    private const string RepositoryAttributeName =
        "SQLSharp.Generator.Respository.RepositoryAttribute";

    private const string QueryAttributeName = "SQLSharp.Generator.Respository.QueryAttribute";

    private const string ParameterAttributeName =
        "SQLSharp.Generator.Respository.ParameterAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource(
                "RepositoryAttribute.g.cs",
                SourceText.From(SourceGenerationHelper.RepositoryAttribute, Encoding.UTF8));
            ctx.AddSource(
                "QueryAttribute.g.cs",
                SourceText.From(SourceGenerationHelper.QueryAttribute, Encoding.UTF8));
            ctx.AddSource(
                "ParameterAttribute.g.cs",
                SourceText.From(SourceGenerationHelper.ParameterAttribute, Encoding.UTF8));
        });

        var typesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                RepositoryAttributeName,
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

        INamedTypeSymbol? repositoryAttribute =
            compilation.GetTypeByMetadataName(RepositoryAttributeName);
        if (repositoryAttribute is null) return;

        INamedTypeSymbol? queryAttribute = compilation.GetTypeByMetadataName(QueryAttributeName);
        if (queryAttribute is null) return;

        INamedTypeSymbol? parameterAttribute =
            compilation.GetTypeByMetadataName(ParameterAttributeName);
        if (parameterAttribute is null) return;

        foreach (INamedTypeSymbol? typeSymbol in typesToGenerate)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (typeSymbol is null)
            {
                continue;
            }

            RepositoryToGenerate repositoryToGenerate = GetRepositoryToGenerate(
                typeSymbol,
                repositoryAttribute,
                queryAttribute,
                parameterAttribute);

            var sourceCode = SourceGenerationHelper.GenerateRepositoryClass(repositoryToGenerate);
            context.AddSource($"Repository.{repositoryToGenerate.InterfaceName}.g.cs",
                SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private static RepositoryToGenerate GetRepositoryToGenerate(
        INamedTypeSymbol typeSymbol,
        INamedTypeSymbol repositoryAttribute,
        INamedTypeSymbol queryAttribute,
        INamedTypeSymbol parameterAttribute)
    {
        return new RepositoryToGenerate(
            typeSymbol.Name,
            typeSymbol.ContainingNamespace.GetFullNamespaceName(),
            typeSymbol.GetMembers()
                .Select(s => s as IMethodSymbol)
                .Where(m => m is not null)
                .Select(m =>
                    RepositoryMethod.FromMethodSymbol(m!, queryAttribute, parameterAttribute))
                .Where(m => m is not null)
                .Select(m => m!)
                .ToImmutableArray());
    }
}

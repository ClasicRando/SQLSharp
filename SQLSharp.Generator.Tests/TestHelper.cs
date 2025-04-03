using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SQLSharp.Generator.Result;

namespace SQLSharp.Generator.Tests;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
        IEnumerable<PortableExecutableReference> references =
        [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        ];

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references);

        var generator = new FromRowGenerator();
        
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator)
            .RunGenerators(compilation);

        return Verifier.Verify(driver)
            .UseDirectory("Snapshots");
    }
}

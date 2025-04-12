using System.Collections.Immutable;

namespace SQLSharp.Generator.Repository;

public record RepositoryToGenerate
{
    public string InterfaceName { get; }
    public string Namespace { get; }
    public ImmutableArray<RepositoryMethod> Methods { get; }

    public RepositoryToGenerate(
        string interfaceName,
        string ns,
        ImmutableArray<RepositoryMethod> methods)
    {
        InterfaceName = interfaceName;
        Namespace = ns;
        Methods = methods;
    }
}

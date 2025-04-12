namespace SQLSharp.Generator.Repository;

[AttributeUsage(validOn: AttributeTargets.Interface)]
public sealed class RepositoryAttribute : Attribute
{
    public Type[] ParameterTypes { get; }
    
    public RepositoryAttribute(params Type[] types)
    {
        ParameterTypes = types;
    }
}

[Repository]
public interface ITemp
{
    
}

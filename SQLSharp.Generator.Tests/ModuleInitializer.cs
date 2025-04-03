using System.Runtime.CompilerServices;

namespace SQLSharp.Generator.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
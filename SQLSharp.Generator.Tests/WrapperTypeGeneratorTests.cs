namespace SQLSharp.Generator.Tests;

[UsesVerify]
public class WrapperTypeGeneratorTests
{
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_StructTypeWithConstructorArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal readonly partial struct BytesWrapper(byte[] inner)
                              {
                                  internal byte[] Inner { get; } = inner;
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_StructTypeWithConstructorNonArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal readonly partial struct IntWrapper(int inner)
                              {
                                  internal int Inner { get; } = inner;
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_StructTypeWithFieldNonArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal readonly partial struct IntWrapper(int inner)
                              {
                                  internal readonly int _inner = inner;
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_StructTypeWithInitPropertyArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal readonly partial struct BytesWrapper
                              {
                                  internal byte[] Inner { get; init; }
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_StructTypeWithInitPropertyNonArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal readonly partial struct IntWrapper
                              {
                                  internal int Inner { get; init; }
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_ClassTypeWithConstructorArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal partial class BytesWrapper(byte[] inner)
                              {
                                  internal byte[] Inner { get; } = inner;
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_ClassTypeWithConstructorNonArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal partial class IntWrapper(int inner)
                              {
                                  internal int Inner { get; } = inner;
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_ClassTypeWithFieldNonArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal partial class IntWrapper(int inner)
                              {
                                  internal readonly int _inner = inner;
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_ClassTypeWithInitPropertyArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal partial class BytesWrapper
                              {
                                  internal byte[] Inner { get; init; }
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_GenerateWrapperIDbDecodeImplementationCorrectly_when_ClassTypeWithInitPropertyNonArrayInnerType()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal partial class IntWrapper
                              {
                                  internal int Inner { get; init; }
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_ProduceCompilerError_when_MultipleFieldsAndNoProperties()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal readonly partial struct IntWrapper
                              {
                                  internal int _inner;
                                  internal byte _other;
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_ProduceCompilerError_when_MultiplePropertiesAndNoFields()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal readonly partial struct IntWrapper
                              {
                                  internal int Inner { get; set; }
                                  internal byte Other { get; set; }
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
    
    [Fact]
    public Task Should_ProduceCompilerError_when_MultipleFieldsAndProperties()
    {
        const string source = """
                              using SQLSharp.Generator.Types;

                              [WrapperTypeAttribute]
                              internal readonly partial struct IntWrapper
                              {
                                  internal int _inner;
                                  internal byte _other;
                                  internal int Inner { get; set; }
                                  internal byte Other { get; set; }
                              }
                              """;

        return TestHelper.VerifyWrapperTypeGenerator(source);
    }
}

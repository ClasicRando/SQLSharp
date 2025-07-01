using System.Data;
using JetBrains.Annotations;

namespace SQLSharp.Generator.Tests;

[TestSubject(typeof(SourceGenerationHelper))]
public class SourceGenerationHelperTest
{
    [Theory]
    [InlineData("System.Boolean", DbType.Boolean)]
    [InlineData("System.Byte", DbType.SByte)]
    [InlineData("System.Int16", DbType.Int16)]
    [InlineData("System.UInt16", DbType.UInt16)]
    [InlineData("System.Int32", DbType.Int32)]
    [InlineData("System.UInt32", DbType.UInt32)]
    [InlineData("System.Int64", DbType.Int64)]
    [InlineData("System.UInt64", DbType.UInt64)]
    [InlineData("System.Byte[]", DbType.Binary)]
    [InlineData("System.Char", DbType.String)]
    [InlineData("System.Char[]", DbType.String)]
    [InlineData("System.String", DbType.String)]
    [InlineData("System.Decimal", DbType.Decimal)]
    [InlineData("System.Single", DbType.Single)]
    [InlineData("System.Double", DbType.Double)]
    [InlineData("System.DateTime", DbType.DateTime)]
    [InlineData("System.DateTimeOffset", DbType.DateTimeOffset)]
    [InlineData("System.Guid", DbType.Guid)]
    public void GetDbType_Should_ReturnExpectedTypeName(string parameter, DbType dbType)
    {
        var dbTypeName = SourceGenerationHelper.GetDbType(parameter);
        Assert.NotNull(dbTypeName);
        Assert.True(Enum.TryParse(
            dbTypeName.Replace("DbType.", "").AsSpan(),
            false,
            out DbType actualDbType));
        Assert.Equal(dbType, actualDbType);
    }
}

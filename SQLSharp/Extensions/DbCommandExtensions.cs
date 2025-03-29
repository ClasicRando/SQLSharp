using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using SQLSharp.Exceptions;
using SQLSharp.Types;

namespace SQLSharp.Extensions;

public static class DbCommandExtensions
{
    internal static void AddParameters(this IDbCommand command, object parameters)
    {
        var type = parameters.GetType();
        if (parameters is IEnumerable<KeyValuePair<string, object?>> keyValuePairs)
        {
            foreach (var pair in keyValuePairs)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = pair.Key;
                EncodeValue(ref parameter, pair.Value);
                command.Parameters.Add(parameter);
            }
        }
        else if (CheckIfAnonymousType(type))
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = propertyInfo.Name;
                var parameterValue = propertyInfo.GetValue(parameters);
                EncodeValue(ref parameter, parameterValue);
                command.Parameters.Add(parameter);
            }
        }
        else
        {
            throw new SqlSharpException(
                $"Parameters supplied in unexpected type. Expected KeyValuePairs or an anonymous type but found {type}");
        }
    }

    private static bool CheckIfAnonymousType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
               && type.IsGenericType
               && type.Name.Contains("AnonymousType")
               && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
               && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    }

    private static void EncodeValue(ref IDbDataParameter parameter, object? value)
    {
        switch (value)
        {
            case null:
                parameter.Value = null;
                break;
            case IDbEncode encode:
                encode.Encode(ref parameter);
                break;
            default:
                parameter.DbType = GetDbType(parameter);
                parameter.Value = value;
                break;
        }
    }

    private static DbType GetDbType(object parameter)
    {
        return parameter switch
        {
            bool => DbType.Boolean,
            byte => DbType.SByte,
            short => DbType.Int16,
            ushort => DbType.UInt16,
            int => DbType.Int32,
            uint => DbType.UInt32,
            long => DbType.Int64,
            ulong => DbType.UInt64,
            byte[] => DbType.Binary,
            char[] => DbType.String,
            string => DbType.String,
            decimal => DbType.Decimal,
            float => DbType.Single,
            double => DbType.Double,
            DateTime => DbType.DateTime,
            DateTimeOffset => DbType.DateTimeOffset,
            Guid => DbType.Guid,
            _ => throw new SqlSharpException(
                $"Could not find DbType for type {parameter.GetType()}"),
        };
    }
}

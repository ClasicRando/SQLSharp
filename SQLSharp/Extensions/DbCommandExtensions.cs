using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using SQLSharp.Command;
using SQLSharp.Exceptions;
using SQLSharp.Types;

namespace SQLSharp.Extensions;

public static class DbCommandExtensions
{
    internal static void AddParameters(this IDbCommand command, object parameters)
    {
        Type type = parameters.GetType();
        switch (parameters)
        {
            case SqlSharpParameters sqlSharpParameters:
                sqlSharpParameters.AddToCommand(command);
                break;
            case IEnumerable<KeyValuePair<string, object?>> keyValuePairs:
            {
                foreach (var pair in keyValuePairs)
                {
                    IDbDataParameter parameter = command.CreateParameter();
                    parameter.ParameterName = pair.Key;
                    EncodeValue(ref parameter, pair.Value);
                    command.Parameters.Add(parameter);
                }

                break;
            }
            default:
            {
                if (CheckIfAnonymousType(type))
                {
                    foreach (PropertyInfo propertyInfo in type.GetProperties())
                    {
                        IDbDataParameter parameter = command.CreateParameter();
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

                break;
            }
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
                if (GetDbType(parameter) is {} dbType)
                {
                    parameter.DbType = dbType;
                }
                parameter.Value = value;
                break;
        }
    }

    private static DbType? GetDbType(object parameter)
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
            _ => null,
        };
    }
}

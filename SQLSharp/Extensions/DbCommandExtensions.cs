using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using SQLSharp.Command;
using SQLSharp.Exceptions;
using SQLSharp.Types;

namespace SQLSharp.Extensions;

internal static class DbCommandExtensions
{
    /// <summary>
    /// Add parameters to this command object. This will only succeed if the parameters are a:
    /// <list type="bullet">
    ///     <item><see cref="SqlSharpParameters"/></item>
    ///     <item><see cref="KeyValuePair{TKey,TValue}"/>s</item>
    ///     <item>an anonymous type</item>
    /// </list>
    /// </summary>
    /// <param name="command">command to add parameters to</param>
    /// <param name="parameters">opaque container for command parameters</param>
    /// <exception cref="SqlSharpException">
    /// if the parameters type cannot be used to populate a command
    /// </exception>
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
                if (IsAnonymousType(type))
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

    private static bool IsAnonymousType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
               && type.IsGenericType
               && type.Name.Contains("AnonymousType")
               && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
               && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    }

    /// <summary>
    /// Encodes a value of any type into the parameter. When the value is of type
    /// <see cref="IDbEncode"/>, this method defers to that method call. Otherwise, the value is
    /// checked to find a compatible base SQL type to associate the parameter to and finally the
    /// value is added as is to the parameter.
    /// </summary>
    /// <param name="parameter">parameter to add the value</param>
    /// <param name="value">opaque value to add to the parameter</param>
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
                if (GetDbType(parameter) is { } dbType)
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

    /// <summary>
    /// Initiate a command with the provided <paramref name="commandProperties"/>
    /// </summary>
    /// <param name="command">command to initialize</param>
    /// <param name="commandProperties">properties to apply to the command</param>
    /// <typeparam name="TConnection">connection type</typeparam>
    /// <typeparam name="TTransaction">transaction type</typeparam>
    internal static void PrepareCommand<TConnection, TTransaction>(
        this IDbCommand command,
        SqlSharpCommand<TConnection, TTransaction> commandProperties)
        where TConnection : IDbConnection
        where TTransaction : IDbTransaction
    {
        command.CommandText = commandProperties.Query;
        command.CommandType = commandProperties.CommandType;
        command.Transaction = commandProperties.Transaction;
        if (commandProperties.QueryTimeout.HasValue)
        {
            command.CommandTimeout = commandProperties.QueryTimeout.Value;
        }

        if (commandProperties.Parameters is not null)
        {
            command.AddParameters(commandProperties.Parameters);
        }
    }
}

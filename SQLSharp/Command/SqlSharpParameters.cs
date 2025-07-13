using System.Data;
using SQLSharp.Exceptions;

namespace SQLSharp.Command;

/// <summary>
/// Collection for dynamic query parameters or when OUTPUT parameter values are required. Create a
/// new empty instance and add each parameter using <see cref="Add"/>. To extract parameter values
/// use <see cref="Get{T}"/>. Just note that OUTPUT parameters are not populated until query
/// execution is complete.
/// </summary>
public class SqlSharpParameters
{
    private class SqlSharpParameter(
        string name,
        object? value,
        ParameterDirection parameterDirection,
        DbType? dbType,
        int? size,
        byte? precision,
        byte? scale)
    {
        public string Name { get; init; } = name;
        public object? Value { get; } = value;
        public ParameterDirection ParameterDirection { get; } = parameterDirection;
        public DbType? DbType { get; } = dbType;
        public int? Size { get; } = size;
        public byte? Precision { get; } = precision;
        public byte? Scale { get; } = scale;
        public IDbDataParameter? DbParameter { get; set; }
    }

    private readonly Dictionary<string, SqlSharpParameter> _parameters = new();

    /// <summary>
    /// Add another parameter to this collection of query parameters
    /// </summary>
    /// <param name="name">name of parameters, '@' or ':' can be omitted</param>
    /// <param name="value">parameters value (if any)</param>
    /// <param name="parameterDirection">
    /// parameter value direction, defaults to input only, OUTPUT parameters value can be acquired
    /// after query execution using <see cref="Get{T}"/>
    /// </param>
    /// <param name="dbType">
    /// database type used for the parameter, only specify when the type cannot be inferred
    /// </param>
    /// <param name="size">size of variable sized fields (e.g. char, varchar, etc.)</param>
    /// <param name="precision">numeric type precision</param>
    /// <param name="scale">numeric type scale</param>
    public void Add(
        string name,
        object? value,
        ParameterDirection parameterDirection = ParameterDirection.Input,
        DbType? dbType = null,
        int? size = null,
        byte? precision = null,
        byte? scale = null)
    {
        _parameters[CleanParameterName(name)] = new SqlSharpParameter(
            name: name,
            value: value,
            parameterDirection: parameterDirection,
            dbType: dbType,
            size: size,
            precision: precision,
            scale: scale);
    }

    /// <summary>
    /// Extract the specified parameter's value. If the parameter is an OUTPUT variable then the
    /// updated value is returned. Otherwise, the original input value is returned.
    /// </summary>
    /// <param name="name">parameter name</param>
    /// <typeparam name="T">type of the return value</typeparam>
    /// <returns>
    /// The parameter's value, note that for DBNull.Value or an initial null value:
    /// <list type="bullet">
    ///     <item>reference types return null even when ? is omitted from the type parameter</item>
    ///     <item>nullable value types return null</item>
    ///     <item>non-null value types throw an exception</item>
    /// </list>
    /// </returns>
    /// <exception cref="SqlSharpException">
    /// <list type="bullet">
    ///     <item>if the parameter name cannot be found</item>
    ///     <item>if the actual value cannot be cast to the desired type</item>
    ///     <item>
    ///         if <typeparamref name="T"/> is a non-null value type and the underlining value is
    ///         DBNull.Value or null
    ///     </item>
    /// </list>
    /// </exception>
    public T Get<T>(string name)
    {
        if (!_parameters.TryGetValue(CleanParameterName(name), out SqlSharpParameter? parameter))
        {
            throw new SqlSharpException($"Cannot find parameter with name = '{name}'");
        }
        
        var value = parameter.DbParameter is null
            ? parameter.Value
            : parameter.DbParameter.Value;
        if (value != DBNull.Value && value != null)
        {
            if (value is T output)
            {
                return output;
            }
            throw new SqlSharpException(
                $"Cannot cast parameter '{name}' of type {value.GetType()} to {typeof(T)}");
        }
        
        if (default(T) is not null)
        {
            throw new SqlSharpException(
                $"Attempted to cast value of DbNull from '{name}' to a non-nullable value. Note! " +
                "If this field is an OUTPUT parameter then those fields are not populated until " +
                "after the query has been fully completed.");
        }
        return default!;
    }

    /// <summary>
    /// Add all parameters to the supplied command
    /// </summary>
    /// <param name="command">command to add parameters to</param>
    internal void AddToCommand(IDbCommand command)
    {
        foreach (var kvp in _parameters)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = kvp.Key;
            parameter.Value = kvp.Value.Value ?? DBNull.Value;
            parameter.Direction = kvp.Value.ParameterDirection;
            if (kvp.Value.DbType is {} dbType)
            {
                parameter.DbType = dbType;
            }
            if (kvp.Value.Size is {} size)
            {
                parameter.Size = size;
            }
            if (kvp.Value.Precision is {} precision)
            {
                parameter.Precision = precision;
            }
            if (kvp.Value.Scale is {} scale)
            {
                parameter.Scale = scale;
            }
            kvp.Value.DbParameter = parameter;
            command.Parameters.Add(parameter);
        }
    }

    /// <summary>
    /// Clean a parameter name to ensure it's uniform across all database drivers. This removes
    /// leading '@', ':' and '?' characters from the name.
    /// </summary>
    /// <param name="name">initial parameter name</param>
    /// <returns>Cleaned parameter name for usage within the internal dictionary</returns>
    private static string CleanParameterName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (string.IsNullOrWhiteSpace(name) || name.Length == 1)
        {
            return name;
        }

        return name[0] switch
        {
            '@' or ':' or '?' => name[1..],
            _ => name,
        };
    }
}

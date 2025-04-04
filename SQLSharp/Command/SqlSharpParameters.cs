using System.Data;
using SQLSharp.Exceptions;

namespace SQLSharp.Command;

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

    public T Get<T>(string name)
    {
        SqlSharpParameter parameter = _parameters[CleanParameterName(name)];
        var value = parameter.DbParameter is null
            ? parameter.Value
            : parameter.DbParameter.Value;
        if (value != DBNull.Value)
        {
            return (T)value!;
        }
        
        if (default(T) is not null)
        {
            throw new SqlSharpException(
                "Attempted to cast a DbNull to a non-nullable value. Note! If this field is " +
                "an OUTPUT parameter then those fields are not populated until after the " +
                "query has been fully completed.");
        }
        return default!;
    }

    internal void AddToCommand(IDbCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
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
        }
    }

    private static string CleanParameterName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (string.IsNullOrEmpty(name))
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

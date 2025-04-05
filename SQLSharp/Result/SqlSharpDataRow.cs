using SQLSharp.Exceptions;

namespace SQLSharp.Result;

internal class SqlSharpDataRow : IDataRow
{
    private readonly List<string> _fieldNames;
    private readonly object[] _values;

    public SqlSharpDataRow(List<string> fieldNames, object[] values)
    {
        _fieldNames = fieldNames ?? throw new ArgumentNullException(nameof(fieldNames));
        _values = values ?? throw new ArgumentNullException(nameof(values));
    }
    
    public int IndexOf(string fieldName)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        var index = _fieldNames.IndexOf(fieldName);
        if (index != -1)
        {
            return index;
        }
        
        var fieldNames = string.Join(",", _fieldNames.Select(n => $"\"{n}\""));
        throw new SqlSharpException(
            $"Could not find field '{fieldName}' in result. Fields names are, {fieldNames}");
    }

    public object this[int index] => _values[index];
}

using System.Data;
using SQLSharp.Exceptions;

namespace SQLSharp.Result;

/// <summary>
/// Implementation of <see cref="IDataRow"/> as a light wrapper over <see cref="IDataRecord"/>.
/// Decodes any extracted DBNull.Value values as null.
/// </summary>
internal sealed class SqlSharpDataRow : IDataRow
{
    private readonly IDataRecord _dataRecord;

    public SqlSharpDataRow(IDataRecord dataRecord)
    {
        _dataRecord = dataRecord;
    }
    
    public int IndexOf(string fieldName)
    {
        var index = _dataRecord.GetOrdinal(fieldName);
        if (index != -1)
        {
            return index;
        }

        var fieldNames = string.Join(
            ",",
            Enumerable.Range(0, _dataRecord.FieldCount)
                .Select(i => $"\"{_dataRecord.GetName(i)}\""));
        throw new SqlSharpException(
            $"Could not find field '{fieldName}' in result. Fields names are, {fieldNames}");
    }

    public object? this[int index]
    {
        get
        {
            var value = _dataRecord[index];
            return value is DBNull ? null : value;
        }
    }
}

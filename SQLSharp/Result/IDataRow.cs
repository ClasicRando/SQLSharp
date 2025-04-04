using SQLSharp.Exceptions;

namespace SQLSharp.Result;

public interface IDataRow
{
    public int IndexOf(string fieldName);

    public object this[int index] { get; }

    public object this[string fieldName] => this[IndexOf(fieldName)];
    
    public T GetField<T>(int index)
    {
        var value = this[index];
        switch (value)
        {
            case DBNull:
                return default!;
            case T v:
                return v;
            default:
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception e)
                {
                    throw new SqlSharpException(
                        $"Cannot convert field value of {value.GetType()} into {typeof(T)}",
                        e);
                }
        }
    }
    
    public T GetFieldNotNull<T>(int index)
    {
        
        var value = this[index];
        switch (value)
        {
            case DBNull:
                throw SqlSharpException.NullField(index);
            case T v:
                return v;
            default:
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception e)
                {
                    throw new SqlSharpException(
                        $"Cannot convert field value of {value.GetType()} into {typeof(T)}",
                        e);
                }
        }
    }

    public T GetField<T>(string fieldName) => GetField<T>(IndexOf(fieldName));

    public T GetFieldNotNull<T>(string fieldName) => GetFieldNotNull<T>(IndexOf(fieldName));
}
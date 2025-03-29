using SQLSharp.Exceptions;

namespace SQLSharp.Result;

public interface IDataRow
{
    public int IndexOf(string fieldName);

    public object? this[int index] { get; }

    public object? this[string fieldName] => this[IndexOf(fieldName)];

    public T? GetFieldAsClass<T>(int index) where T : class
    {
        var value = this[index];
        switch (value)
        {
            case null or DBNull:
                return null;
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

    public T? GetFieldAsClass<T>(string fieldName) where T : class =>
        GetFieldAsClass<T>(IndexOf(fieldName));

    public T GetFieldAsClassNotNull<T>(int index) where T : class
    {
        return GetFieldAsClass<T>(index) ?? throw SqlSharpException.MissingOrNullField(index);
    }

    public T GetFieldAsClassNotNull<T>(string fieldName) where T : class
    {
        return GetFieldAsClass<T>(fieldName) ??
               throw SqlSharpException.MissingOrNullField(fieldName);
    }

    public T? GetField<T>(int index) where T : struct
    {
        var value = this[index];
        switch (value)
        {
            case null or DBNull:
                return null;
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

    public T? GetField<T>(string fieldName) where T : struct => GetField<T>(IndexOf(fieldName));

    public T GetFieldNotNull<T>(int index) where T : struct
    {
        return GetField<T>(index) ?? throw SqlSharpException.MissingOrNullField(index);
    }

    public T GetFieldNotNull<T>(string fieldName) where T : struct
    {
        return GetField<T>(fieldName) ?? throw SqlSharpException.MissingOrNullField(fieldName);
    }
}
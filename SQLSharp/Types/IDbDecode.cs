using SQLSharp.Result;

namespace SQLSharp.Types;

public interface IDbDecode<out TResult>
{
    public static abstract TResult Decode(IDataRow row, int column);
}

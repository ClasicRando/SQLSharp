namespace SQLSharp.Result;

public interface IFromRow<out TSelf>
{
    public static abstract TSelf FromRow(IDataRow row);
}

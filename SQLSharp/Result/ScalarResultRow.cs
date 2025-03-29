using SQLSharp.Types;

namespace SQLSharp.Result;

internal class ScalarResultRow<TDecoder, TResult> : IFromRow<ScalarResultRow<TDecoder, TResult>> where TDecoder : IDbDecode<TResult>
{
    public TResult Inner { get; }

    private ScalarResultRow(TResult inner)
    {
        Inner = inner;
    }
    
    public static ScalarResultRow<TDecoder, TResult> FromRow(IDataRow row)
    {
        return new ScalarResultRow<TDecoder, TResult>(TDecoder.Decode(row, 0));
    }
}

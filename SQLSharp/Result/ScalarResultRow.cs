using SQLSharp.Types;

namespace SQLSharp.Result;

/// <summary>
/// Internal implementation of <see cref="IFromRow{TSelf}"/> for extracting a scalar value that
/// implements <see cref="IDbDecode{TResult}"/> from a row. Defers to the Decode method for parsing
/// the first column.
/// </summary>
/// <typeparam name="TDecoder">
/// Decoder type that parses into <typeparamref name="TResult"/>
/// </typeparam>
/// <typeparam name="TResult">Final decoded value type</typeparam>
internal class ScalarResultRow<TDecoder, TResult> : IFromRow<ScalarResultRow<TDecoder, TResult>> where TDecoder : IDbDecode<TResult>
{
    public TResult? Inner { get; }

    private ScalarResultRow(TResult? inner)
    {
        Inner = inner;
    }
    
    public static ScalarResultRow<TDecoder, TResult> FromRow(IDataRow row)
    {
        return new ScalarResultRow<TDecoder, TResult>(TDecoder.Decode(row, 0));
    }
}

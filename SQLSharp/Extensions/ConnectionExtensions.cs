using System.Data;
using SQLSharp.Command;
using SQLSharp.Exceptions;
using SQLSharp.Result;
using SQLSharp.Types;

namespace SQLSharp.Extensions;

public static class ConnectionExtensions
{
    public static T? QueryScalarDecode<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IDbDecode<T>
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryScalarImpl<T, T>(command);
    }

    public static T? QueryScalarValue<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null)
        where
        T : struct
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryScalarImpl<AnyDbDecode<T>, T?>(command);
    }

    public static T? QueryScalar<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null)
        where
        T : class
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryScalarImpl<AnyRefDbDecode<T>, T?>(command);
    }

    public static T QuerySingle<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        return connection.QuerySingleOrNull<T>(
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType) ?? throw new SqlSharpException("Expected exactly 1 row but found none");
    }

    public static T? QuerySingleOrNull<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        var rows = QueryImpl<T>(command);
        using var enumerator = rows.GetEnumerator();
        var result = enumerator.MoveNext() ? enumerator.Current : default;
        if (enumerator.MoveNext())
        {
            throw new SqlSharpException("Expected exactly 1 row but found more than 1");
        }

        return result;
    }

    public static T QueryFirst<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        return connection.QueryFirstOrNull<T>(
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType) ?? throw new SqlSharpException("Expected at least 1 row but found zero");
    }

    public static T? QueryFirstOrNull<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryFirstOrNullImpl<T>(command);
    }

    public static IEnumerable<T> Query<T>(
        this IDbConnection connection,
        string query,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null) where T : IFromRow<T>
    {
        SqlSharpCommand<IDbConnection, IDbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryImpl<T>(command);
    }

    private static IEnumerable<T> QueryImpl<T>(
        SqlSharpCommand<IDbConnection, IDbTransaction> sqlSharpCommand) where T : IFromRow<T>
    {
        var wasClosed = sqlSharpCommand.Connection.State == ConnectionState.Closed;
        IDbCommand? command = null;
        IDataReader? reader = null;

        try
        {
            if (wasClosed)
            {
                sqlSharpCommand.Connection.Open();
            }

            command = sqlSharpCommand.Connection.CreateCommand();
            command.CommandText = sqlSharpCommand.Query;
            command.CommandType = sqlSharpCommand.CommandType;
            command.Transaction = sqlSharpCommand.Transaction;
            command.CommandTimeout = sqlSharpCommand.QueryTimeout;

            if (sqlSharpCommand.Parameters is not null)
            {
                command.AddParameters(sqlSharpCommand.Parameters);
            }

            reader = command.ExecuteReader();
            if (!reader.Read()) yield break;

            var schemaTable = reader.GetSchemaTable();
            var columnNameQuery = from DataRow column in schemaTable!.Rows
                select column.Field<string>("ColumnName");
            List<string> fieldNames = columnNameQuery.ToList();

            do
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                var row = new SqlSharpDataRow(fieldNames, values);
                yield return T.FromRow(row);
            } while (reader.Read());

            while (reader.NextResult())
            {
            }

            reader.Dispose();
            reader = null;
        }
        finally
        {
            if (reader is not null)
            {
                if (!reader.IsClosed)
                {
                    try
                    {
                        command?.Cancel();
                    }
                    catch
                    {
                        // ignored
                    }

                    reader.Dispose();
                }
            }

            if (wasClosed)
            {
                sqlSharpCommand.Connection.Close();
            }

            command?.Parameters.Clear();
            command?.Dispose();
        }
    }

    private static T? QueryFirstOrNullImpl<T>(
        SqlSharpCommand<IDbConnection, IDbTransaction> command) where T : IFromRow<T>
    {
        var rows = QueryImpl<T>(command);
        using var enumerator = rows.GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current : default;
    }

    private static TResult? QueryScalarImpl<TDecoder, TResult>(
        SqlSharpCommand<IDbConnection, IDbTransaction> command) where TDecoder : IDbDecode<TResult>
    {
        var row = QueryFirstOrNullImpl<ScalarResultRow<TDecoder, TResult>>(command);
        return row is null ? default : row.Inner;
    }
}
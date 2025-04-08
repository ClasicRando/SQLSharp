using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using SQLSharp.Command;
using SQLSharp.Exceptions;
using SQLSharp.Result;
using SQLSharp.Types;

namespace SQLSharp.Extensions;

public static class AsyncConnectionExtensions
{
    public static Task<T> QueryScalarDecodeAsync<T>(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
        where
        T : IDbDecode<T>
    {
        SqlSharpCommand<DbConnection, DbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryScalarAsyncImpl<T, T>(command, cancellationToken);
    }

    public static async Task<T> QueryScalarAsync<T>(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        SqlSharpCommand<DbConnection, DbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return await QueryScalarAsyncImpl<AnyDbDecode<T>, T>(command, cancellationToken);
    }

    public static async Task<T> QuerySingleAsync<T>(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
        where
        T : IFromRow<T>
    {
        return await connection.QuerySingleOrNullAsync<T>(
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType,
            cancellationToken
        ) ?? throw new SqlSharpException("Expected exactly 1 row but found none");
    }

    public static async Task<T?> QuerySingleOrNullAsync<T>(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default) where T : IFromRow<T>
    {
        var rows = connection.QueryAsync<T>(
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType,
            cancellationToken
        );
        await using var enumerator = rows.GetAsyncEnumerator(cancellationToken);
        T? result = await enumerator.MoveNextAsync() ? enumerator.Current : default;
        if (await enumerator.MoveNextAsync())
        {
            throw new SqlSharpException("Expected exactly 1 row but found more than 1");
        }

        return result;
    }

    public static async Task<T> QueryFirstAsync<T>(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default) where T : IFromRow<T>
    {
        return await connection.QueryFirstOrNullAsync<T>(
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType,
            cancellationToken
        ) ?? throw new SqlSharpException("Expected at least 1 row but found zero");
    }

    public static Task<T?> QueryFirstOrNullAsync<T>(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default) where T : IFromRow<T>
    {
        SqlSharpCommand<DbConnection, DbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryFirstOrNullAsyncImpl<T>(command, cancellationToken);
    }

    public static IAsyncEnumerable<T> QueryAsync<T>(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default) where T : IFromRow<T>
    {
        SqlSharpCommand<DbConnection, DbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return QueryAsyncImpl<T>(command, cancellationToken);
    }

    public static async Task<List<T>> QueryAllAsync<T>(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default) where T : IFromRow<T>
    {
        var result = await QueryAsync<T>(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType,
            cancellationToken)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return result;
    }

    private static async IAsyncEnumerable<T> QueryAsyncImpl<T>(
        SqlSharpCommand<DbConnection, DbTransaction> sqlSharpAsyncCommand,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where T : IFromRow<T>
    {
        var wasClosed = sqlSharpAsyncCommand.Connection.State == ConnectionState.Closed;
        DbCommand? command = null;
        DbDataReader? reader = null;

        try
        {
            if (wasClosed)
            {
                await sqlSharpAsyncCommand.Connection.OpenAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            command = sqlSharpAsyncCommand.Connection.CreateCommand();
            command.CommandText = sqlSharpAsyncCommand.Query;
            command.CommandType = sqlSharpAsyncCommand.CommandType;
            command.Transaction = sqlSharpAsyncCommand.Transaction;
            command.CommandTimeout = sqlSharpAsyncCommand.QueryTimeout;

            if (sqlSharpAsyncCommand.Parameters is not null)
            {
                command.AddParameters(sqlSharpAsyncCommand.Parameters);
            }

            reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) yield break;

            DataTable? schemaColumns =
                await reader.GetSchemaTableAsync(cancellationToken).ConfigureAwait(false);
            var columnNameQuery = from DataRow column in schemaColumns!.Rows
                select column.Field<string>("ColumnName");
            List<string> fieldNames = columnNameQuery.ToList();

            do
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                var row = new SqlSharpDataRow(fieldNames, values);
                yield return T.FromRow(row);
            } while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false));

            while (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false))
            {
            }

            await reader.DisposeAsync().ConfigureAwait(false);
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

                    await reader.DisposeAsync().ConfigureAwait(false);
                }
            }

            if (wasClosed)
            {
                await sqlSharpAsyncCommand.Connection.CloseAsync().ConfigureAwait(false);
            }

            if (command is not null)
            {
                command.Parameters.Clear();
                await command.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    public static Task<int> ExecuteAsync(
        this DbConnection connection,
        string query,
        object? parameters = null,
        DbTransaction? transaction = null,
        int? queryTimeout = null,
        CommandType? commandType = null,
        CancellationToken cancellationToken = default)
    {
        SqlSharpCommand<DbConnection, DbTransaction> command = new(
            connection,
            query,
            parameters,
            transaction,
            queryTimeout,
            commandType);
        return ExecuteAsync(command, cancellationToken);
    }

    private static async Task<int> ExecuteAsync(
        SqlSharpCommand<DbConnection, DbTransaction> sqlSharpAsyncCommand,
        CancellationToken cancellationToken = default)
    {
        var wasClosed = sqlSharpAsyncCommand.Connection.State == ConnectionState.Closed;
        DbCommand? command = null;

        try
        {
            if (wasClosed)
            {
                await sqlSharpAsyncCommand.Connection.OpenAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            command = sqlSharpAsyncCommand.Connection.CreateCommand();
            command.CommandText = sqlSharpAsyncCommand.Query;
            command.CommandType = sqlSharpAsyncCommand.CommandType;
            command.Transaction = sqlSharpAsyncCommand.Transaction;
            command.CommandTimeout = sqlSharpAsyncCommand.QueryTimeout;

            if (sqlSharpAsyncCommand.Parameters is not null)
            {
                command.AddParameters(sqlSharpAsyncCommand.Parameters);
            }

            return await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (wasClosed)
            {
                await sqlSharpAsyncCommand.Connection.CloseAsync().ConfigureAwait(false);
            }

            if (command is not null)
            {
                command.Parameters.Clear();
                await command.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private static async Task<T?> QueryFirstOrNullAsyncImpl<T>(
        SqlSharpCommand<DbConnection, DbTransaction> command,
        CancellationToken cancellationToken) where T : IFromRow<T>
    {
        var rows = QueryAsyncImpl<T>(command, cancellationToken);
        await using var enumerator = rows.GetAsyncEnumerator(cancellationToken);
        return await enumerator.MoveNextAsync().ConfigureAwait(false)
            ? enumerator.Current
            : default;
    }

    private static async Task<TResult> QueryScalarAsyncImpl<TDecoder, TResult>(
        SqlSharpCommand<DbConnection, DbTransaction> command,
        CancellationToken cancellationToken) where TDecoder : IDbDecode<TResult>
    {
        var row = await QueryFirstOrNullAsyncImpl<ScalarResultRow<TDecoder, TResult>>(
            command,
            cancellationToken);
        if (row is null)
        {
            return default!;
        }
        return row.Inner ?? default!;
    }
}
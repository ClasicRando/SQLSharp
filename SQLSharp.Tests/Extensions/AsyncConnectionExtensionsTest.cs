using JetBrains.Annotations;
using SQLSharp.Exceptions;
using SQLSharp.Extensions;
using SQLSharp.Result;
using SQLSharp.Types;

namespace SQLSharp.Tests.Extensions;

[TestSubject(typeof(AsyncConnectionExtensions))]
[Collection("PostgresDb")]
public class AsyncConnectionExtensionsTest
{
    private readonly PostgresDbFixture _fixture;
    private static readonly Row ExpectedRow = new(
        Guid.Parse("485a6d30-265f-4486-9aef-a6398241eec9"),
        "Test");

    public AsyncConnectionExtensionsTest(PostgresDbFixture fixture)
    {
        _fixture = fixture;
    }

    private record struct IntWrapper(int Inner) : IDbDecode<IntWrapper>
    {
        public static IntWrapper Decode(IDataRow row, int column)
        {
            return new IntWrapper(row.GetFieldNotNull<int>(column));
        }
    }
    
    [UsedImplicitly]
    private record Row(Guid Id, string Name) : IFromRow<Row>
    {
        public static Row FromRow(IDataRow row)
        {
            return new Row(
                row.GetFieldNotNull<Guid>("id"),
                row.GetFieldNotNull<string>("name"));
        }
    }

    [Fact(DisplayName = "QueryScalarAsync should extract first value when not decode type")]
    public async Task QueryScalarAsync()
    {
        var actual = await _fixture.Connection.QueryScalarAsync<int>("SELECT 1");
        
        Assert.Equal(1, actual);
    }
    
    [Fact(DisplayName = "QueryScalarDecodeAsync should extract first value when decode type")]
    public async Task QueryScalarDecodeAsync()
    {
        var actual = await _fixture.Connection.QueryScalarDecodeAsync<IntWrapper>("SELECT 1");
        
        Assert.Equal(1, actual.Inner);
    }

    [Collection("PostgresDb")]
    public class QuerySingleAsync
    {
        private readonly PostgresDbFixture _fixture;

        public QuerySingleAsync(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QuerySingleAsync should extract first row when single row")]
        public async Task Test1()
        {
            var actual = await _fixture.Connection.QuerySingleAsync<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
        
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QuerySingleAsync should throw when multiple rows returned")]
        public async Task Test2()
        {
            var e = await Assert.ThrowsAsync<SqlSharpException>(async () =>
            {
                await _fixture.Connection.QuerySingleAsync<Row>(
                    """
                    SELECT gen_random_uuid() AS "id", '' "name"
                    UNION ALL
                    SELECT gen_random_uuid() AS "id", '' "name"
                    """);
            });
            
            Assert.Equal("Expected exactly 1 row but found more than 1", e.Message);
        }
        
        [Fact(DisplayName = "QuerySingleAsync should throw when no rows returned")]
        public async Task Test3()
        {
            var e = await Assert.ThrowsAsync<SqlSharpException>(async () =>
            {
                await _fixture.Connection.QuerySingleAsync<Row>(
                    """
                    SELECT *
                    FROM (SELECT gen_random_uuid() AS "id", null "name") t
                    WHERE t.name IS NOT NULL
                    """);
            });
            
            Assert.Equal("Expected exactly 1 row but found none", e.Message);
        }
    }

    [Collection("PostgresDb")]
    public class QuerySingleOrNullAsync
    {
        private readonly PostgresDbFixture _fixture;

        public QuerySingleOrNullAsync(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QuerySingleOrNullAsync should extract first row when single row")]
        public async Task Test1()
        {
            var actual = await _fixture.Connection.QuerySingleOrNullAsync<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
        
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QuerySingleOrNullAsync should throw when multiple rows returned")]
        public async Task Test2()
        {
            var e = await Assert.ThrowsAsync<SqlSharpException>(async () =>
            {
                await _fixture.Connection.QuerySingleOrNullAsync<Row>(
                    """ 
                    SELECT gen_random_uuid() AS "id", '' "name"
                    UNION ALL
                    SELECT gen_random_uuid() AS "id", '' "name"
                    """);
            });
            
            Assert.Equal("Expected exactly 1 row but found more than 1", e.Message);
        }
        
        [Fact(DisplayName = "QuerySingleOrNullAsync should return null when no rows returned")]
        public async Task Test3()
        {
            var actual = await _fixture.Connection.QuerySingleOrNullAsync<Row>(
                    """
                    SELECT *
                    FROM (SELECT gen_random_uuid() AS "id", null "name") t
                    WHERE t.name IS NOT NULL
                    """);
            
            Assert.Null(actual);
        }
    }

    [Collection("PostgresDb")]
    public class QueryFirstAsync
    {
        private readonly PostgresDbFixture _fixture;

        public QueryFirstAsync(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QueryFirstAsync should extract first row when single row")]
        public async Task Test1()
        {
            var actual = await _fixture.Connection.QueryFirstAsync<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
        
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QueryFirstAsync should extract first row when multiple rows")]
        public async Task Test2()
        {
            var actual = await _fixture.Connection.QueryFirstAsync<Row>(
                    """
                    SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                    UNION ALL
                    SELECT gen_random_uuid() AS "id", '' "name"
                    """);
            
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QueryFirstAsync should throw when no rows returned")]
        public async Task Test3()
        {
            var e = await Assert.ThrowsAsync<SqlSharpException>(async () =>
            {
                await _fixture.Connection.QueryFirstAsync<Row>(
                    """
                    SELECT *
                    FROM (SELECT gen_random_uuid() AS "id", null "name") t
                    WHERE t.name IS NOT NULL
                    """);
            });
            
            Assert.Equal("Expected at least 1 row but found zero", e.Message);
        }
    }
    
    [Collection("PostgresDb")]
    public class QueryFirstOrNullAsync
    {
        private readonly PostgresDbFixture _fixture;

        public QueryFirstOrNullAsync(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QueryFirstOrNullAsync should extract first row when single row")]
        public async Task Test1()
        {
            var actual = await _fixture.Connection.QueryFirstOrNullAsync<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
        
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QueryFirstOrNullAsync should extract first row when multiple rows")]
        public async Task Test2()
        {
            var actual = await _fixture.Connection.QueryFirstOrNullAsync<Row>(
                """
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                UNION ALL
                SELECT gen_random_uuid() AS "id", '' "name"
                """);
            
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QueryFirstOrNullAsync should return null when no rows returned")]
        public async Task Test3()
        {
            var actual = await _fixture.Connection.QueryFirstOrNullAsync<Row>(
                    """
                    SELECT *
                    FROM (SELECT gen_random_uuid() AS "id", null "name") t
                    WHERE t.name IS NOT NULL
                    """);
            
            Assert.Null(actual);
        }
    }
    
    [Collection("PostgresDb")]
    public class QueryAsync
    {
        private readonly PostgresDbFixture _fixture;

        public QueryAsync(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QueryAsync should extract first row when single row")]
        public async Task Test1()
        {
            var iter = _fixture.Connection.QueryAsync<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
            await VerifyRows(iter, 1);
        }
        
        [Fact(DisplayName = "QueryAsync should extract all rows when multiple rows")]
        public async Task Test2()
        {
            var iter = _fixture.Connection.QueryAsync<Row>(
                """
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                UNION ALL
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                """);
            await VerifyRows(iter, 2);
        }
        
        [Fact(DisplayName = "QueryFirstOrNullAsync should return null when no rows returned")]
        public async Task Test3()
        {
            var actual = await _fixture.Connection.QueryFirstOrNullAsync<Row>(
                """
                SELECT *
                FROM (SELECT gen_random_uuid() AS "id", null "name") t
                WHERE t.name IS NOT NULL
                """);
            
            Assert.Null(actual);
        }

        private static async Task VerifyRows(IAsyncEnumerable<Row> iter, int expectedCount)
        {
            var count = 0;
            await foreach (Row row in iter)
            {
                count++;
                Assert.Equal(ExpectedRow, row);
            }
            Assert.Equal(expectedCount, count);
        }
    }
    
    
    [Collection("PostgresDb")]
    public class QueryAllAsync
    {
        private readonly PostgresDbFixture _fixture;

        public QueryAllAsync(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QueryAllAsync should extract first row when single row")]
        public async Task Test1()
        {
            var list = await _fixture.Connection.QueryAllAsync<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
            Assert.Single(list);
            Assert.Equal(ExpectedRow, list[0]);
        }
        
        [Fact(DisplayName = "QueryAllAsync should extract all rows when multiple rows")]
        public async Task Test2()
        {
            var list = await _fixture.Connection.QueryAllAsync<Row>(
                """
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                UNION ALL
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                """);
            Assert.Equal(2, list.Count);
            Assert.Equal(ExpectedRow, list[0]);
            Assert.Equal(ExpectedRow, list[1]);
        }
        
        [Fact(DisplayName = "QueryAllAsync should return empty list when no rows returned")]
        public async Task Test3()
        {
            var actual = await _fixture.Connection.QueryAllAsync<Row>(
                """
                SELECT *
                FROM (SELECT gen_random_uuid() AS "id", null "name") t
                WHERE t.name IS NOT NULL
                """);
            Assert.Empty(actual);
        }
    }
}
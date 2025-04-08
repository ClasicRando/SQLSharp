using JetBrains.Annotations;
using SQLSharp.Exceptions;
using SQLSharp.Extensions;
using SQLSharp.Result;
using SQLSharp.Types;

namespace SQLSharp.Tests.Extensions;

[Collection("PostgresDb")]
public class ConnectionExtensionsTest
{
    private readonly PostgresDbFixture _fixture;
    private static readonly Row ExpectedRow = new(
        Guid.Parse("485a6d30-265f-4486-9aef-a6398241eec9"),
        "Test");

    public ConnectionExtensionsTest(PostgresDbFixture fixture)
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

    [Fact(DisplayName = "QueryScalar should extract first value when not decode type")]
    public void QueryScalar()
    {
        var actual = _fixture.Connection.QueryScalar<int>("SELECT 1");
        
        Assert.Equal(1, actual);
    }
    
    [Fact(DisplayName = "QueryScalarDecode should extract first value when decode type")]
    public void QueryScalarDecode()
    {
        var actual = _fixture.Connection.QueryScalarDecode<IntWrapper>("SELECT 1");
        
        Assert.Equal(1, actual.Inner);
    }

    [Collection("PostgresDb")]
    public class QuerySingle
    {
        private readonly PostgresDbFixture _fixture;

        public QuerySingle(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QuerySingle should extract first row when single row")]
        public void Test1()
        {
            var actual = _fixture.Connection.QuerySingle<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
        
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QuerySingle should throw when multiple rows returned")]
        public void Test2()
        {
            var e = Assert.Throws<SqlSharpException>( () =>
            {
                _fixture.Connection.QuerySingle<Row>(
                    """
                    SELECT gen_random_uuid() AS "id", '' "name"
                    UNION ALL
                    SELECT gen_random_uuid() AS "id", '' "name"
                    """);
            });
            
            Assert.Equal("Expected exactly 1 row but found more than 1", e.Message);
        }
        
        [Fact(DisplayName = "QuerySingle should throw when no rows returned")]
        public void Test3()
        {
            var e = Assert.Throws<SqlSharpException>( () =>
            {
                _fixture.Connection.QuerySingle<Row>(
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
    public class QuerySingleOrNull
    {
        private readonly PostgresDbFixture _fixture;

        public QuerySingleOrNull(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QuerySingleOrNull should extract first row when single row")]
        public void Test1()
        {
            var actual = _fixture.Connection.QuerySingleOrNull<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
        
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QuerySingleOrNull should throw when multiple rows returned")]
        public void Test2()
        {
            var e = Assert.Throws<SqlSharpException>( () =>
            {
                _fixture.Connection.QuerySingleOrNull<Row>(
                    """ 
                    SELECT gen_random_uuid() AS "id", '' "name"
                    UNION ALL
                    SELECT gen_random_uuid() AS "id", '' "name"
                    """);
            });
            
            Assert.Equal("Expected exactly 1 row but found more than 1", e.Message);
        }
        
        [Fact(DisplayName = "QuerySingleOrNull should return null when no rows returned")]
        public void Test3()
        {
            var actual = _fixture.Connection.QuerySingleOrNull<Row>(
                    """
                    SELECT *
                    FROM (SELECT gen_random_uuid() AS "id", null "name") t
                    WHERE t.name IS NOT NULL
                    """);
            
            Assert.Null(actual);
        }
    }

    [Collection("PostgresDb")]
    public class QueryFirst
    {
        private readonly PostgresDbFixture _fixture;

        public QueryFirst(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QueryFirst should extract first row when single row")]
        public void Test1()
        {
            var actual = _fixture.Connection.QueryFirst<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
        
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QueryFirst should extract first row when multiple rows")]
        public void Test2()
        {
            var actual = _fixture.Connection.QueryFirst<Row>(
                    """
                    SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                    UNION ALL
                    SELECT gen_random_uuid() AS "id", '' "name"
                    """);
            
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QueryFirst should throw when no rows returned")]
        public void Test3()
        {
            var e = Assert.Throws<SqlSharpException>( () =>
            {
                _fixture.Connection.QueryFirst<Row>(
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
    public class QueryFirstOrNull
    {
        private readonly PostgresDbFixture _fixture;

        public QueryFirstOrNull(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QueryFirstOrNull should extract first row when single row")]
        public void Test1()
        {
            var actual = _fixture.Connection.QueryFirstOrNull<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
        
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QueryFirstOrNull should extract first row when multiple rows")]
        public void Test2()
        {
            var actual = _fixture.Connection.QueryFirstOrNull<Row>(
                """
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                UNION ALL
                SELECT gen_random_uuid() AS "id", '' "name"
                """);
            
            Assert.Equal(ExpectedRow, actual);
        }
        
        [Fact(DisplayName = "QueryFirstOrNull should return null when no rows returned")]
        public void Test3()
        {
            var actual = _fixture.Connection.QueryFirstOrNull<Row>(
                    """
                    SELECT *
                    FROM (SELECT gen_random_uuid() AS "id", null "name") t
                    WHERE t.name IS NOT NULL
                    """);
            
            Assert.Null(actual);
        }
    }
    
    [Collection("PostgresDb")]
    public class Query
    {
        private readonly PostgresDbFixture _fixture;

        public Query(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "Query should extract first row when single row")]
        public void Test1()
        {
            var iter = _fixture.Connection.Query<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
            VerifyRows(iter, 1);
        }
        
        [Fact(DisplayName = "Query should extract all rows when multiple rows")]
        public void Test2()
        {
            var iter = _fixture.Connection.Query<Row>(
                """
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                UNION ALL
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                """);
            VerifyRows(iter, 2);
        }
        
        [Fact(DisplayName = "QueryFirstOrNull should return null when no rows returned")]
        public void Test3()
        {
            var actual = _fixture.Connection.QueryFirstOrNull<Row>(
                """
                SELECT *
                FROM (SELECT gen_random_uuid() AS "id", null "name") t
                WHERE t.name IS NOT NULL
                """);
            
            Assert.Null(actual);
        }

        private static void VerifyRows(IEnumerable<Row> iter, int expectedCount)
        {
            var count = 0;
            foreach (Row row in iter)
            {
                count++;
                Assert.Equal(ExpectedRow, row);
            }
            Assert.Equal(expectedCount, count);
        }
    }
    
    
    [Collection("PostgresDb")]
    public class QueryAll
    {
        private readonly PostgresDbFixture _fixture;

        public QueryAll(PostgresDbFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact(DisplayName = "QueryAll should extract first row when single row")]
        public void Test1()
        {
            var list = _fixture.Connection.QueryAll<Row>(
                "SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS \"id\", 'Test' \"name\"");
            Assert.Single(list);
            Assert.Equal(ExpectedRow, list[0]);
        }
        
        [Fact(DisplayName = "QueryAll should extract all rows when multiple rows")]
        public void Test2()
        {
            var list = _fixture.Connection.QueryAll<Row>(
                """
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                UNION ALL
                SELECT '485a6d30-265f-4486-9aef-a6398241eec9'::uuid AS "id", 'Test' "name"
                """);
            Assert.Equal(2, list.Count);
            Assert.Equal(ExpectedRow, list[0]);
            Assert.Equal(ExpectedRow, list[1]);
        }
        
        [Fact(DisplayName = "QueryAll should return empty list when no rows returned")]
        public void Test3()
        {
            var actual = _fixture.Connection.QueryAll<Row>(
                """
                SELECT *
                FROM (SELECT gen_random_uuid() AS "id", null "name") t
                WHERE t.name IS NOT NULL
                """);
            Assert.Empty(actual);
        }
    }
}
using System.Data.Common;

namespace System.Linq.Sql.Sqlite
{
#if DEBUG
    public
#else
    internal
#endif
    class SqliteQueryableProvider : SqlQueryableProvider
    {
        public SqliteQueryableProvider(DbConnection connection)
            : base(connection, new SqliteExpressionVisitor())
        { }
    }
}

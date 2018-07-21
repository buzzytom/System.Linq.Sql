using System.Data.Common;
using System.Linq.Expressions;

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
            : base(connection, new SqliteQueryVisitor())
        { }

        public override IQueryable CreateQuery(Expression expression)
        {
            return new SqliteQueryable(this, expression);
        }
    }
}

using System;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace LinqSql.Queryable
{
    using Expressions;

    public class SqlQueryableProvider : IQueryProvider
    {
        private readonly DbConnection connection = null;
        private readonly SqlExpressionVisitor visitor = null;

        public SqlQueryableProvider(DbConnection connection)
            : this(connection, new SqlExpressionVisitor())
        { }

        protected SqlQueryableProvider(DbConnection connection, SqlExpressionVisitor visitor)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (visitor == null)
                throw new ArgumentNullException(nameof(visitor));

            this.connection = connection;
            this.visitor = visitor;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new SqlQueryable(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)CreateQuery(expression);
        }

        public object Execute(Expression expression)
        {
            if (expression is ASourceExpression source)
                return connection.ExecuteQuery(source, visitor);
            else
                throw new InvalidOperationException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }
    }
}

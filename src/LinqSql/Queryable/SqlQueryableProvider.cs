using System.Data.Common;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
#if DEBUG
    public
#else
    internal
#endif
    class SqlQueryableProvider : IQueryProvider
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

        public virtual IQueryable CreateQuery(Expression expression)
        {
            return new SqlQueryable(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)CreateQuery(expression);
        }

        public object Execute(Expression expression)
        {
            // Translate the expression tree
            expression = SqlTranslatorVisitor.Translate(expression);

            // Execute the query
            if (expression is ASourceExpression source)
                return connection.ExecuteQuery(source, visitor);

            throw new NotSupportedException("The expression could not be converted to sql.");
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }
    }
}

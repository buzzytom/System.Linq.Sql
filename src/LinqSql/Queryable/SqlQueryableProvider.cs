using System.Linq;
using System.Linq.Expressions;

namespace LinqSql.Queryable
{
    public class SqlQueryableProvider : IQueryProvider
    {
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
            return SqlQueryableContext.Execute(expression, false);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)SqlQueryableContext.Execute(expression, enumerable: typeof(TResult).Name == "IEnumerable`1");
        }
    }
}

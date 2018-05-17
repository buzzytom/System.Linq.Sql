using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Sql
{
    /// <summary>
    /// Provides a set of static methods for querying <see cref="SqlQueryable"/> instances.
    /// </summary>
    public static class SqlQueryableHelper
    {
        /// <summary>
        /// Correlates the elements of two sequences based on the specifed predicate.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="predicate">A function to test each element of each sequence for a condition.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <param name="comparer">The type of join this call represents.</param>
        /// <returns>An <see cref="IQueryable{Record}"/> that has elements obtained by performing a join on the two sequences.</returns>
        /// <exception cref="ArgumentNullException">outer, inner, predicate or resultSelector is null.</exception>
        public static IQueryable<Record> Join(this IQueryable<Record> outer, IQueryable<Record> inner, Expression<Func<Record, Record, bool>> predicate, Expression<Func<Record, Record, Record>> resultSelector, JoinType joinType = JoinType.Inner)
        {
            if (outer == null)
                throw new ArgumentNullException(nameof(outer));
            if (inner == null)
                throw new ArgumentNullException(nameof(inner));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return outer.Provider.CreateQuery<Record>(
                Expression.Call(
                    null,
                    (MethodInfo)MethodBase.GetCurrentMethod(),
                    new Expression[]
                    {
                        outer.Expression,
                        inner.Expression,
                        Expression.Quote(predicate),
                        Expression.Quote(resultSelector),
                        Expression.Constant(joinType)
                    }));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Linq.Sql.Queryable
{
    /// <summary>
    /// Provides a set of static methods for querying <see cref="SqlQueryable"/> instances.
    /// </summary>
    public static class SqlQueryableHelper
    {
        /// <summary>
        /// Correlates the elements of two sequences based on the specifed predicate.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="predicate">A function to test each element of each sequence for a condition.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <param name="comparer">The type of join this call represents.</param>
        /// <returns>An <see cref="IQueryable{T}"/> that has elements of type <see cref="{TResult}"/> obtained by performing a join on the two sequences.</returns>
        /// <exception cref="ArgumentNullException">outer, inner, predicate or resultSelector is null.</exception>
        public static IQueryable<TResult> Join<TOuter, TInner,  TResult>(this IQueryable<TOuter> outer, IQueryable<TInner> inner, Expression<Func<TOuter, TInner, bool>> predicate, Expression<Func<TOuter, TInner, Record>> resultSelector, JoinType joinType = JoinType.Inner)
        {
            if (outer == null)
                throw new ArgumentNullException(nameof(outer));
            if (inner == null)
                throw new ArgumentNullException(nameof(inner));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            return outer.Provider.CreateQuery<TResult>(
                Expression.Call(
                    null,
                    MethodBase.GetCurrentMethod(),
                    new Expression[]
                    {
                        outer.Expression,
                        inner.Expression,
                        Expression.Quote(predicate),
                        Expression.Quote(resultSelector),
                        Expression.Constant(joinType);
        }
                    ));
        }
}
}

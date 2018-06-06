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

        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default equality comparer.
        /// </summary>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <returns>true if the source sequence contains an element that has the specified value; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        /// <remarks>If more than one row, table or column is returned, they will be ignored.</remarks>
        public static bool Contains(this IQueryable<Record> source, Expression<Func<Record, object>> selector, object value)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            // Create and execute the query
            IQueryable<Record> records = source.Provider.CreateQuery<Record>(
                Expression.Call(
                    null,
                    (MethodInfo)MethodBase.GetCurrentMethod(),
                    new Expression[]
                    {
                        source.Expression,
                        selector,
                        Expression.Convert(Expression.Constant(value), typeof(object))
                    }));

            // Get the scalar value
            return records.GetScalar<bool>();
        }

        /// <summary>
        /// Computes the average of a sequence of values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        public static int Average(this IQueryable<Record> source, Expression<Func<Record, int>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Computes the average of a sequence of values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        public static long Average(this IQueryable<Record> source, Expression<Func<Record, long>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Computes the average of a sequence of values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        public static decimal Average(this IQueryable<Record> source, Expression<Func<Record, decimal>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Computes the average of a sequence of values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        public static float Average(this IQueryable<Record> source, Expression<Func<Record, float>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Computes the average of a sequence of values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate the average of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The average of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null</exception>
        public static double Average(this IQueryable<Record> source, Expression<Func<Record, double>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Returns the number of elements in the specified sequence that satisfies a condition.
        /// </summary>
        /// <param name="source">The sequence with elements to be counted.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The number of elements in the sequence that satisfies the condition in the predicate function.</returns>
        /// <exception cref="ArgumentNullException">The source argument is null.</exception>
        /// <exception cref="OverflowException">The number of elements in source (after applying the predicate) is larger than <see cref="System.Int32.MaxValue"/>.</exception>
        public static int Count(this IQueryable<Record> source, Expression<Func<Record, bool>> predicate = null)
        {
            // Map the predicate to true when null
            if (predicate == null)
                predicate = record => true;

            return source.EvaluateScalar<int>((MethodInfo)MethodBase.GetCurrentMethod(), predicate);
        }

        /// <summary>
        /// Selects a value for each element of a sequence and returns the maximum resulting value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the function represented by selector.</typeparam>
        /// <param name="source">A sequence of values to determine the maximum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The maximum value in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static TResult Max<TResult>(this IQueryable<Record> source, Expression<Func<Record, TResult>> selector)
        {
            return EvaluateAggregate(source, selector, ((Func<IQueryable<Record>, Expression<Func<Record, TResult>>, TResult>)Max).Method);
        }

        /// <summary>
        /// Selects a value for each element of a sequence and returns the minimum resulting value.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the function represented by selector.</typeparam>
        /// <param name="source">A sequence of values to determine the minimum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The minimum value in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static TResult Min<TResult>(this IQueryable<Record> source, Expression<Func<Record, TResult>> selector)
        {
            return EvaluateAggregate(source, selector, ((Func<IQueryable<Record>, Expression<Func<Record, TResult>>, TResult>)Min).Method);
        }

        /// <summary>
        /// Selects a value for each element of a sequence and returns the sum of all the values.
        /// </summary>
        /// <param name="source">A sequence of values to determine the sum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The sum of all the selected values in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static int Sum(this IQueryable<Record> source, Expression<Func<Record, int>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Selects a value for each element of a sequence and returns the sum of all the values.
        /// </summary>
        /// <param name="source">A sequence of values to determine the sum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The sum of all the selected values in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static long Sum(this IQueryable<Record> source, Expression<Func<Record, long>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Selects a value for each element of a sequence and returns the sum of all the values.
        /// </summary>
        /// <param name="source">A sequence of values to determine the sum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The sum of all the selected values in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static decimal Sum(this IQueryable<Record> source, Expression<Func<Record, decimal>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Selects a value for each element of a sequence and returns the sum of all the values.
        /// </summary>
        /// <param name="source">A sequence of values to determine the sum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The sum of all the selected values in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static float Sum(this IQueryable<Record> source, Expression<Func<Record, float>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Selects a value for each element of a sequence and returns the sum of all the values.
        /// </summary>
        /// <param name="source">A sequence of values to determine the sum of.</param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>The sum of all the selected values in the sequence.</returns>
        /// <exception cref="ArgumentNullException">source or selector is null.</exception>
        public static double Sum(this IQueryable<Record> source, Expression<Func<Record, double>> selector)
        {
            return EvaluateAggregate(source, selector, MethodBase.GetCurrentMethod());
        }

        private static T EvaluateAggregate<T>(this IQueryable<Record> source, Expression<Func<Record, T>> selector, MethodBase method)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            // TODO - Add support for something similar to this to the translators field selector so a default selector can be created.
            //if (selector == null)
            //    selector = record => (T)record.FirstTableColumnValue();

            return source.EvaluateScalar<T>((MethodInfo)method, selector);
        }

        private static T EvaluateScalar<T>(this IQueryable<Record> source, MethodInfo method, Expression parameter)
        {
            return source
                .EvaluateQuery(method, parameter)
                .GetScalar<T>();
        }

        private static IQueryable<Record> EvaluateQuery(this IQueryable<Record> source, MethodInfo method, Expression parameter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Provider.CreateQuery<Record>(
                Expression.Call(
                    null,
                    method,
                    new Expression[]
                    {
                        source.Expression,
                        parameter
                    }));
        }
    }
}

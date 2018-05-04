using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql.Queryable
{
    using Expressions;

    /// <summary>
    /// <see cref="SqlQueryable"/> is an implementation of <see cref="IOrderedQueryable{Record}"/> which is used to execute sql queries in a linq like manner.
    /// </summary>
    public class SqlQueryable : IOrderedQueryable<Record>
    {
        private readonly SqlQueryableProvider provider = null;
        private readonly Expression expression = null;

        /// <summary>
        /// Initializes a new instance of <see cref="SqlQueryable"/> from the specified table information.
        /// </summary>
        /// <param name="connection">The database connection to query from.</param>
        /// <param name="table">The name of the table to be queried.</param>
        /// <param name="fields">The fields that exist on the specified table.</param>
        public SqlQueryable(DbConnection connection, string table, IEnumerable<string> fields)
            : this(connection, table, table, fields)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SqlQueryable"/> from the specified table information.
        /// </summary>
        /// <param name="connection">The database connection to query from.</param>
        /// <param name="table">The name of the table to be queried.</param>
        /// <param name="alias">An alias to give the table for use in query filtering.</param>
        /// <param name="fields">The fields that exist on the specified table.</param>
        public SqlQueryable(DbConnection connection, string table, string alias, IEnumerable<string> fields)
        {
            expression = new TableExpression(table, alias, fields);
            provider = new SqlQueryableProvider(connection);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SqlQueryable"/>, with the specified provider and expression.
        /// </summary>
        /// <param name="provider">The provider which the query should use.</param>
        /// <param name="expression">The root expression of the query.</param>
        internal SqlQueryable(SqlQueryableProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            this.provider = provider;
            this.expression = expression;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{Record}"/> object that can be used to iterate through the collection.</returns>
        public IEnumerator<Record> GetEnumerator()
        {
            return Provider
                .Execute<IEnumerable<Record>>(expression)
                .GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{Record}"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            //return GetEnumerator();
            return Provider
                .Execute<IEnumerable>(Expression)
                .GetEnumerator();
        }

        // ----- Properties ----- //

        /// <summary>Gets the type of the element(s) that are returned when the expression tree associated with this <see cref="SqlQueryable"/> is executed.</summary>
        public Type ElementType => typeof(Record);

        /// <summary>Gets the query provider that is associated with this data source.</summary>
        public IQueryProvider Provider => provider;

        /// <summary>Gets the expression tree that is associated with the instance this <see cref="SqlQueryable"/>.</summary>
        public Expression Expression => expression;
    }
}

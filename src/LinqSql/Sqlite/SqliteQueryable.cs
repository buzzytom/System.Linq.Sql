using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

namespace System.Linq.Sql.Sqlite
{
    /// <summary>
    /// <see cref="SqliteQueryable"/> is an SQLite specific implementation of <see cref=SqlQueryable"/> which is used to execute sql queries in a linq like manner.
    /// </summary>
    public class SqliteQueryable : SqlQueryable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SqliteQueryable"/> from the specified table information.
        /// </summary>
        /// <param name="connection">The database connection to query from.</param>
        /// <param name="table">The name of the table to be queried.</param>
        /// <param name="fields">The fields that exist on the specified table.</param>
        public SqliteQueryable(DbConnection connection, string table, IEnumerable<string> fields)
            : this(connection, table, table, fields)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SqliteQueryable"/> from the specified table information.
        /// </summary>
        /// <param name="connection">The database connection to query from.</param>
        /// <param name="table">The name of the table to be queried.</param>
        /// <param name="alias">An alias to give the table for use in query filtering.</param>
        /// <param name="fields">The fields that exist on the specified table.</param>
        public SqliteQueryable(DbConnection connection, string table, string alias, IEnumerable<string> fields)
            : this(new SqliteQueryableProvider(connection), new TableExpression(table, alias, fields))
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SqliteQueryable"/>, with the specified provider and expression.
        /// </summary>
        /// <param name="provider">The provider which the query should use.</param>
        /// <param name="expression">The root expression of the query.</param>
        protected internal SqliteQueryable(SqliteQueryableProvider provider, Expression expression)
            : base(provider, expression)
        { }
    }
}

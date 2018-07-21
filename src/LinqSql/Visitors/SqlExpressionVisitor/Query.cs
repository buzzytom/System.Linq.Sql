using System.Collections.Generic;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="Query"/> represents an sql query to a database as well as any parameters needed by the query.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Creates a new instance of <see cref="Query"/> with the specified sql and parameters.
        /// </summary>
        /// <param name="sql">The sql of the query.</param>
        public Query(string sql, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("Cannot be null or whitespace.", nameof(sql));

            Sql = sql;
            Parameters = parameters.ToArray();
        }

        // ----- Properties ----- //

        /// <summary>Gets the sql this query represents.</summary>
        public string Sql { private set; get; }

        /// <summary>Gets the parameter inputs that are used in the query.</summary>
        public IEnumerable<KeyValuePair<string, object>> Parameters { private set; get; }
    }
}

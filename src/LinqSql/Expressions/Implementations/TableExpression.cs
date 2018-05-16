using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents a record selection of a database table.
    /// </summary>
    public class TableExpression : ASourceExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TableExpression"/>, selecting the specified fields from the specified table.
        /// </summary>
        /// <param name="table">The name of the table in the database to query.</param>
        /// <param name="alias">The alias name the <see cref="TableExpression"/> should expose for other queries.</param>
        public TableExpression(string table, string alias, IEnumerable<string> fields)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException("Cannot be whitespace.", nameof(alias));
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));

            Table = table;
            Alias = alias;
            Fields = new FieldExpressions(this, alias, fields);
            Expressions = new ASourceExpression[0];
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitTable(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the physical name of table.</summary>
        public string Table { get; } = null;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public string Alias { get; } = null;

        /// <summary>Gets the fields exposed by the table.</summary>
        public override FieldExpressions Fields { get; } = null;

        /// <summary>Gets the child expressions of this source.</summary>
        public override IEnumerable<ASourceExpression> Expressions { get; } = null;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqSql.Expressions
{
    /// <summary>
    /// Represents a record selection of a database table.
    /// </summary>
    public class TableExpression : ASourceExpression
    {
        private readonly string table = null;
        private readonly string alias = null;
        private readonly IEnumerable<FieldExpression> fields = null;

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

            this.table = table;
            this.alias = alias;
            this.fields = fields.Select(x => new FieldExpression(this, x, x));
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlExpressionVisitor sqlVisitor)
                return sqlVisitor.VisitTable(this);
            else
                return base.Accept(visitor);
        }

        // ----- Properties ----- //

        /// <summary>Gets the physical name of table.</summary>
        public string Table => table;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public string Alias => alias;

        /// <summary>Gets the fields exposed by the table.</summary>
        public override IEnumerable<FieldExpression> Fields => fields;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqSql.Expressions
{
    /// <summary>
    /// Represents a record selection of a database table.
    /// </summary>
    public class TableExpression : AExpression
    {
        private readonly string table = null;
        private readonly string alias = null;
        private readonly string[] fields = null;

        /// <summary>
        /// Initializes a new instance of <see cref="TableExpression"/>, selecting the specified fields from the specified table.
        /// </summary>
        /// <param name="fields">The fields from each record of the table to query.</param>
        /// <param name="table">The name of the table in the database to query.</param>
        /// <param name="alias">The alias name the <see cref="TableExpression"/> should expose for other queries.</param>
        public TableExpression(IEnumerable<string> fields, string table, string alias)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Cannot be whitespace.", nameof(table));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException("Cannot be whitespace.", nameof(alias));

            this.table = table;
            this.alias = alias;
            this.fields = fields.ToArray();
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        public override AExpression Accept(AExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }
        
        // ----- Properties ----- //

        /// <summary>Gets the physical name of table.</summary>
        public string Table => table;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public string Alias => alias;

        /// <summary>Gets a collection of the fields present on the table.</summary>
        public IEnumerable<string> Fields => fields;
    }
}

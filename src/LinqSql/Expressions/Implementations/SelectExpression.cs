using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqSql.Expressions
{
    /// <summary>
    /// Represents the field selection on a source expression.
    /// </summary>
    public class SelectExpression : ASourceExpression
    {
        private readonly ASourceExpression source = null;
        private readonly FieldExpressions fields = new FieldExpressions();

        /// <summary>
        /// Initializes a new instance of <see cref="TableExpression"/>, selecting the specified fields from the specified table.
        /// </summary>
        /// <param name="fields">The fields from each record of the table to query.</param>
        /// <param name="table">The name of the table in the database to query.</param>
        /// <param name="alias">The alias name the <see cref="TableExpression"/> should expose for other queries.</param>
        public SelectExpression(ASourceExpression source, IEnumerable<string> fields)
        {
            if (source == null)
                throw new ArgumentNullException();
            if (fields == null)
                throw new ArgumentNullException();
            if (!fields.Any())
                throw new ArgumentException("There must be at least one field specified in a select query.", nameof(fields));

            this.source = source;
            this.fields = new FieldExpressions(source, fields);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        public override void Accept(AExpressionVisitor visitor)
        {
            visitor.VisitSelect(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets inner source of the expression.</summary>
        public ASourceExpression Source => source;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public override IEnumerable<FieldExpression> Fields => fields;
    }
}

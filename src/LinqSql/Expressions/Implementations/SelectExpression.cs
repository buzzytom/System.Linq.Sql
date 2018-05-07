﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// Represents the field selection on a source expression.
    /// </summary>
    public class SelectExpression : ASourceExpression
    {
        private readonly ASourceExpression source = null;
        private readonly FieldExpressions fields = null;

        /// <summary>
        /// Initializes a new instance of <see cref="SelectExpression"/> selecting all the fields from the specified source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        public SelectExpression(ASourceExpression source)
            : this(source, source?.Fields)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SelectExpression"/> selecting the specified fields from the source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        /// <param name="fields">The fields to select from the source.</param>
        public SelectExpression(ASourceExpression source, IEnumerable<FieldExpression> fields)
        {
            if (source == null)
                throw new ArgumentNullException();
            if (fields == null)
                throw new ArgumentNullException();
            if (!fields.Any())
                throw new ArgumentException("There must be at least one field specified in a select query.", nameof(fields));

            this.source = source;
            this.fields = new FieldExpressions(fields);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitSelect(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets inner source of the expression.</summary>
        public ASourceExpression Source => source;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public override FieldExpressions Fields => fields;
    }
}

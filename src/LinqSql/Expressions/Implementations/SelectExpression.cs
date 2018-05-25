using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents the field selection on a source expression.
    /// </summary>
    public class SelectExpression : ASourceExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SelectExpression"/> selecting all the fields from the specified source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        /// /// <param name="take">The number of fields to take from the source. Values less than zero indicate all rows.</param>
        /// <param name="skip">The number of fields to ignore on the source before reading rows.</param>
        public SelectExpression(ASourceExpression source, int take = -1, int skip = 0)
            : this(source, source?.Fields, take, skip)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="SelectExpression"/> selecting the specified fields from the source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        /// <param name="fields">The fields to select from the source.</param>
        /// <param name="take">The number of fields to take from the source. Values less than zero indicate all rows.</param>
        /// <param name="skip">The number of fields to ignore on the source before reading rows.</param>
        public SelectExpression(ASourceExpression source, IEnumerable<FieldExpression> fields, int take = -1, int skip = 0)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (!fields.Any())
                throw new ArgumentException("There must be at least one field specified in a select query.", nameof(fields));

            Source = source;
            Fields = new FieldExpressions(this, fields);
            Expressions = new[] { source };
            Take = take;
            Skip = skip;
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
        public ASourceExpression Source { get; } = null;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public override FieldExpressions Fields { get; } = null;

        /// <summary>Gets the child expressions of this source.</summary>
        public override IEnumerable<ASourceExpression> Expressions { get; } = null;

        /// <summary>Gets the number of rows that will be skipped on the source expression.</summary>
        public int Skip { get; } = 0;

        /// <summary>Gets the number of rows that will be taken from the source expression.</summary>
        public int Take { get; } = -1;
    }
}

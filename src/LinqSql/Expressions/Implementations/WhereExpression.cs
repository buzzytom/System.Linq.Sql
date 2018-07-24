using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents the filter of a query source.
    /// </summary>
    public class WhereExpression : ASourceExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WhereExpression"/> filtering the specified source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        public WhereExpression(ASourceExpression source, APredicateExpression predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            Source = source;
            Predicate = predicate;

            Fields = new FieldExpressions(this, source.Fields);
            Expressions = new[] { source };
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(IQueryVisitor visitor)
        {
            return visitor.VisitWhere(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets inner source of the expression.</summary>
        public ASourceExpression Source { get; } = null;

        /// <summary>Gets the predicate of this epxression.</summary>
        public APredicateExpression Predicate { get; } = null;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public override FieldExpressions Fields { get; } = null;

        /// <summary>Gets the child expressions of this source.</summary>
        public override IEnumerable<ASourceExpression> Expressions { get; } = null;
    }
}

using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents the filter of a query source.
    /// </summary>
    public class WhereExpression : ASourceExpression
    {
        private readonly FieldExpressions fields = null;
        private readonly ASourceExpression source = null;
        private readonly APredicateExpression predicate = null;

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

            this.source = source;
            this.predicate = predicate;

            fields = new FieldExpressions(source, source.Fields);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitWhere(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets inner source of the expression.</summary>
        public ASourceExpression Source => source;

        /// <summary>Gets the predicate of this epxression.</summary>
        public APredicateExpression Predicate => predicate;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public override FieldExpressions Fields => fields;
    }
}

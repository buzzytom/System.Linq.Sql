using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// Represents the filter of a query source.
    /// </summary>
    public class WhereExpression : ASourceExpression
    {
        private readonly ASourceExpression source = null;

        /// <summary>
        /// Initializes a new instance of <see cref="WhereExpression"/> filtering the specified source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        public WhereExpression(ASourceExpression source)
        {
            if (source == null)
                throw new ArgumentNullException();

            this.source = source;
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

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public override FieldExpressions Fields => source.Fields;
    }
}

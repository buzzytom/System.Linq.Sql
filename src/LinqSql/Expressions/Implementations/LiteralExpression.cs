using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="LiteralExpression"/> represents a constant value in the query, these are often translated to query parameters.
    /// </summary>
    public class LiteralExpression : AExpression
    {
        private readonly object value = null;

        /// <summary>
        /// Initializes a new instance of <see cref="LiteralExpression"/>, with the specified value.
        /// </summary>
        public LiteralExpression(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            this.value = value;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitLiteral(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the value of this expression.</summary>
        public object Value => value;

        /// <summary>Gets the static type of the expression that this System.Linq.Expressions.Expression represents.</summary>
        public override Type Type => value.GetType();
    }
}

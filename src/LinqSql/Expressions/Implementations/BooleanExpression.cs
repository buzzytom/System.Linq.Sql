using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="BooleanExpression"/> represents a constant boolean value in the query. Unlike the <see cref="LiteralExpression"/>, these are not typically translated to query parameters.
    /// </summary>
    public class BooleanExpression : APredicateExpression
    {
        private readonly bool value = false;

        /// <summary>
        /// Initializes a new instance of <see cref="LiteralExpression"/>, with the specified value.
        /// </summary>
        public BooleanExpression(bool value)
        {
            this.value = value;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitBoolean(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the value of this expression.</summary>
        public bool Value => value;

        /// <summary>Gets the static type of the expression that this System.Linq.Expressions.Expression represents.</summary>
        public override Type Type => typeof(bool);
    }
}

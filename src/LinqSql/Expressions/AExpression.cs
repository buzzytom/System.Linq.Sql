using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// <see cref="AExpression"/> provides the base class from which concrete query expressions are derived. This is an abstract class. This class extends <see cref="Expression"/>.
    /// </summary>
    public abstract class AExpression : Expression
    {
        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor is ISqlExpressionVisitor sqlVisitor)
                return AcceptSql(sqlVisitor);
            else
                return base.Accept(visitor);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected abstract Expression AcceptSql(ISqlExpressionVisitor visitor);

        // ----- Properties ----- //

        /// <summary>Gets the node type of this System.Linq.Expressions.Expression.</summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>Gets the static type of the expression that this System.Linq.Expressions.Expression represents.</summary>
        public override Type Type => typeof(object);
    }
}

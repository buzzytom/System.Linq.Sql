using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="NullExpression"/> represents a constant null value in the query.
    /// </summary>
    public class NullExpression : AExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NullExpression"/>.
        /// </summary>
        public NullExpression()
        { }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitNull(this);
        }
    }
}

using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// <see cref="AExpression"/> provides the base class from which concrete query expressions are derived. This is an abstract class. This class extends <see cref="Expression"/>.
    /// </summary>
    public abstract class AExpression : Expression
    {
        /// <summary>Gets the node type of this System.Linq.Expressions.Expression.</summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>Gets the static type of the expression that this System.Linq.Expressions.Expression represents.</summary>
        public override Type Type => typeof(object);
    }
}

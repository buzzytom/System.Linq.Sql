using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// <see cref="AExpression"/> provides the base class from which concrete query expressions are derived. This is an abstract class. This class extends <see cref="Expression"/>.
    /// </summary>
    public abstract class AExpression : Expression
    {
        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(object);
    }
}

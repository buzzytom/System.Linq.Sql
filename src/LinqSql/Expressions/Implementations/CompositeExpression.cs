using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="CompositeOperator"/> defines an enumeration of possible comparisons of predicate expressions: And and Or.
    /// </summary>
    public enum CompositeOperator
    {
        And,
        Or,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        NotEqual,
        Equal
    }

    /// <summary>
    /// <see cref="CompositeExpression"/> represents the comparison of two expressions using a <see cref="CompositeOperator"/>.
    /// </summary>
    public class CompositeExpression : APredicateExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CompositeExpression"/>, with the specified left, right and operator arguments.
        /// </summary>
        /// <param name="left">The left hand expression to compare.</param>
        /// <param name="right">The right hand expression to compare.</param>
        /// <param name="type">The operator to use when comparing the left and right hand predicates.</param>
        public CompositeExpression(AExpression left, AExpression right, CompositeOperator type = CompositeOperator.And)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            Left = left;
            Right = right;
            Operator = type;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(IQueryVisitor visitor)
        {
            return visitor.VisitComposite(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the left hand expression.</summary>
        public AExpression Left { get; } = null;

        /// <summary>Gets the right hand expression.</summary>
        public AExpression Right { get; } = null;

        /// <summary>Gets the operator being used to compare the <see cref="Left"/> and <see cref="Right"/> expression.</summary>
        public CompositeOperator Operator { get; } = CompositeOperator.Equal;
    }
}

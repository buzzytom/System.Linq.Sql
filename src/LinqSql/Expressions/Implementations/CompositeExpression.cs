using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// <see cref="CompositeOperator"/> defines an enumeration of possible comparisons of predicate expressions: And and Or.
    /// </summary>
    public enum CompositeOperator
    {
        And,
        Or
    }

    /// <summary>
    /// <see cref="CompositeExpression"/> represents the comparison of two boolean expressions using either And or Or.
    /// </summary>
    public class CompositeExpression : APredicateExpression
    {
        private readonly APredicateExpression left = null;
        private readonly APredicateExpression right = null;
        private readonly CompositeOperator type = CompositeOperator.And;

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeExpression"/>, with the specified left, right and operator arguments.
        /// </summary>
        /// <param name="left">The left hand boolean predicate to compare.</param>
        /// <param name="right">The right hand boolean predicate to compare.</param>
        /// <param name="type">The operator to use when comparing the left and right hand predicates.</param>
        public CompositeExpression(APredicateExpression left, APredicateExpression right, CompositeOperator type = CompositeOperator.And)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            this.left = left;
            this.right = right;
            this.type = type;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitComposite(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the left hand predicate of this expression.</summary>
        public APredicateExpression Left => left;

        /// <summary>Gets the right hand predicate of this expression.</summary>
        public APredicateExpression Right => right;

        /// <summary>Gets the operator being used to compare the <see cref="Left"/> and <see cref="Right"/> expression.</summary>
        public CompositeOperator Operator => type;
    }
}

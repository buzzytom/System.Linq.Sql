using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents an <see cref="APredicateExpression"/> which resolves if a sequence contains a value
    /// </summary>
    public class ContainsExpression : APredicateExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ContainsExpression"/> with the specified values and value expressions.
        /// </summary>
        /// <param name="values">An expression representing a sequence of values.</param>
        /// <param name="value">The expression representing a value that should be searched for in the values.</param>
        public ContainsExpression(AExpression values, AExpression value)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Values = values;
            Value = value;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(IQueryExpressionVisitor visitor)
        {
            return visitor.VisitContains(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the Values of this object.</summary>
        public AExpression Values { get; } = null;

        /// <summary>Gets the Value of this object.</summary>
        public AExpression Value { get; } = null;
    }
}

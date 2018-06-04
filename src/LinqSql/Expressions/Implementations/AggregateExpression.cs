using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// An enumeration of the supported sql aggregation functions.
    /// </summary>
    public enum AggregateFunction
    {
        /// <summary>Will get the numerical average of a numeric sequence.</summary>
        Average,
        /// <summary>Gets the total number of items in the aggregation.</summary>
        Count,
        /// <summary>Gets the largest numerical value in the aggregation.</summary>
        Max,
        /// <summary>Gets the smallest numerical value in the aggregation.</summary>
        Min,
        /// <summary>Gets the total numerical value of adding all the values in the aggregation together.</summary>
        Sum,
        /// <summary>Gets the value of the first item in the aggregation.</summary>
        Top
    }

    /// <summary>
    /// <see cref="AggregateExpression"/> represents the scalar count of a source query expression.
    /// </summary>
    public class AggregateExpression : AExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AggregateExpression"/> with the specified source and function.
        /// </summary>
        /// <param name="source">The source expression to perform the aggregate function on.</param>
        /// <param name="field">The field the aggregate function is applied to.</param>
        /// <param name="value">The type of aggregate operation to perform.</param>
        public AggregateExpression(ASourceExpression source, FieldExpression field, AggregateFunction function)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            Source = source;
            SourceField = field;
            Function = function;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {  
            return visitor.VisitAggregate(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the source expression this expression is operating on.</summary>
        public ASourceExpression Source { get; } = null;

        /// <summary>Gets the field on the <see cref="Source"/> which is having the aggregate function applied to it.</summary>
        public FieldExpression SourceField { get; } = null;

        /// <summary>Gets the function this of this expression.</summary>
        public AggregateFunction Function { get; }
    }
}

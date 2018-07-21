using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents the single field selection on a source expression which represents a single result.
    /// </summary>
    public class ScalarExpression : SelectExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ScalarExpression"/> selecting the single field exposed on the source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        public ScalarExpression(ASourceExpression source)
            : this(source, source?.Fields.Single())
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ScalarExpression"/> selecting the specified field from the source.
        /// </summary>
        /// <param name="source">The source expression to select from.</param>
        /// <param name="field">The field to select from the source.</param>
        public ScalarExpression(ASourceExpression source, FieldExpression field)
            : base(source, field != null ? new[] { field } : throw new ArgumentNullException(nameof(field)))
        { }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(IQueryExpressionVisitor visitor)
        {
            return visitor.VisitScalar(this);
        }
    }
}

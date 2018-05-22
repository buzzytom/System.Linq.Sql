using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// Represents a column selection of a database table.
    /// </summary>
    public class FieldExpression : AFieldExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FieldExpression"/>, selecting the specified field from the specified source.
        /// </summary>
        /// <param name="expression">The fields <see cref="ASourceExpression"/>.</param>
        /// <param name="table">The table or table alias the field is exposed from.</param>
        /// <param name="field">The name of the field on the source.</param>
        /// <param name="source">The optional field expression this instance is mapping.</param>
        public FieldExpression(ASourceExpression expression, string table, string field, AFieldExpression source = null)
            : base(expression, table, field)
        {
            Source = source;
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitField(this);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        public override Expression AcceptDeclarationSql(ISqlExpressionVisitor visitor)
        {
            return visitor.VisitFieldDeclaration(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the optional source field this field is mapping.</summary>
        public AFieldExpression Source { get; } = null;
    }
}

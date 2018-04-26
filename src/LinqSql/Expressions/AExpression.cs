namespace LinqSql.Expressions
{
    /// <summary>
    /// <see cref="AExpression"/> provides the base class from which concrete expressions are derived. This is an abstract class.
    /// </summary>
    public abstract class AExpression
    {
        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        public abstract AExpression Accept(AExpressionVisitor visitor);
    }
}

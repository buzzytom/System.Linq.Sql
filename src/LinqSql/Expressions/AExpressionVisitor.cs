namespace LinqSql.Expressions
{
    /// <summary>
    /// <see cref="AExpressionVisitor"/> represents a visitor for expression trees.
    /// </summary>
    public class AExpressionVisitor
    {
        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression.</returns>
        public AExpression Visit(AExpression expression)
        {
            return expression.Accept(this);
        }
    }
}

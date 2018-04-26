namespace LinqSql.Expressions
{
    /// <summary>
    /// <see cref="AExpressionVisitor"/> represents a visitor for expression trees.
    /// </summary>
    public abstract class AExpressionVisitor
    {
        /// <summary>
        /// Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public void Visit(AExpression expression)
        {
            expression.Accept(this);
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public abstract void VisitTable(TableExpression expression);

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <remarks>Extend this method to add custom expressions to the visitor.</remarks>
        public virtual void VisitExtension(AExpression expression)
        {
        }
    }
}

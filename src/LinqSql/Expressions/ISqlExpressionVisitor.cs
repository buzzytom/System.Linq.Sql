using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions
{
    /// <summary>
    /// <see cref="ISqlExpressionVisitor"/> represents a visitor for expression trees.
    /// </summary>
    public interface ISqlExpressionVisitor
    {
        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        Expression VisitBoolean(BooleanExpression expression);

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        Expression VisitComposite(CompositeExpression expression);

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        Expression VisitField(FieldExpression expression);

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        Expression VisitLiteral(LiteralExpression expression);

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        Expression VisitNull(NullExpression expression);

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        Expression VisitSelect(SelectExpression expression);

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        Expression VisitTable(TableExpression expression);

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        Expression VisitWhere(WhereExpression expression);
    }
}

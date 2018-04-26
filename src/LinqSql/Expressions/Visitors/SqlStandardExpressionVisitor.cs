using System.Text;

namespace LinqSql.Expressions
{
    /// <summary>
    /// <see cref="SqlStandardExpressionVisitor"/> is an implementation of <see cref="AExpressionVisitor"/>, which the visit implementations generate an SQL representation of .
    /// </summary>
    public class SqlStandardExpressionVisitor : AExpressionVisitor
    {
        private StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Creates a new instance of <see cref="SqlStandardExpressionVisitor"/>.
        /// </summary>
        public SqlStandardExpressionVisitor()
        {
        }

        /// <summary>
        /// Generates the sql representation of the specified expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree to generate sql for.</param>
        /// <returns>The sql representation of the specified expression.</returns>
        /// <remarks>This method will clear any state currently executing on the visitor.</remarks>
        public string GenerateSql(AExpression expression)
        {
            builder.Clear();
            expression.Accept(this);
            return builder.ToString();
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public override void VisitTable(TableExpression expression)
        {
            builder.Append($"{expression.Table} as [{expression.Alias}]");
        }

        // ----- Properties ----- //

        /// <summary>Gets the string builder used to form the query.</summary>
        protected StringBuilder Builder => builder;

        /// <summary>Gets the current sql state of the visitor.</summary>
        public string SqlState => builder.ToString();
    }
}

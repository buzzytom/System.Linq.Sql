using System.Linq.Expressions;

namespace LinqSql.Expressions.Tests
{
    public class MockExpressionVisitor : ExpressionVisitor, ISqlExpressionVisitor
    {
        public Expression VisitField(FieldExpression expression)
        {
            FieldVisited = true;
            return expression;
        }

        public Expression VisitSelect(SelectExpression expression)
        {
            SelectVisited = true;
            return expression;
        }

        public Expression VisitTable(TableExpression expression)
        {
            TableVisited = true;
            return expression;
        }

        // ----- Properties ----- //

        public bool FieldVisited { private set; get; } = false;

        public bool SelectVisited { private set; get; } = false;

        public bool TableVisited { private set; get; } = false;
    }
}

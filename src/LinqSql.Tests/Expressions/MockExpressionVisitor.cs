using System.Linq.Expressions;

namespace System.Linq.Sql.Expressions.Tests
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

        public Expression VisitWhere(WhereExpression expression)
        {
            WhereVisited = true;
            return expression;
        }

        // ----- Properties ----- //

        public bool FieldVisited { private set; get; } = false;

        public bool SelectVisited { private set; get; } = false;

        public bool TableVisited { private set; get; } = false;

        public bool WhereVisited { private set; get; } = false;
    }
}

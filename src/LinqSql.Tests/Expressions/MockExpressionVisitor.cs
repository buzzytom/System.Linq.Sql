using System.Linq.Expressions;

namespace System.Linq.Sql.Tests
{
    public class MockExpressionVisitor : ExpressionVisitor, IQueryExpressionVisitor
    {
        public Expression VisitAggregate(AggregateExpression expression)
        {
            AggregateVisited = true;
            return expression;
        }

        public Expression VisitBoolean(BooleanExpression expression)
        {
            BooleanVisited = true;
            return expression;
        }

        public Expression VisitComposite(CompositeExpression expression)
        {
            CompositeVisited = true;
            return expression;
        }

        public Expression VisitContains(ContainsExpression expression)
        {
            ContainsVisited = true;
            return expression;
        }

        public Expression VisitField(FieldExpression expression)
        {
            FieldVisited = true;
            return expression;
        }

        public Expression VisitFieldDeclaration(FieldExpression expression)
        {
            FieldDeclarationVisited = true;
            return expression;
        }

        public Expression VisitJoin(JoinExpression expression)
        {
            JoinVisited = true;
            return expression;
        }

        public Expression VisitLiteral(LiteralExpression expression)
        {
            LiteralVisited = true;
            return expression;
        }

        public Expression VisitNull(NullExpression expression)
        {
            NullVisited = true;
            return expression;
        }

        public Expression VisitScalar(ScalarExpression expression)
        {
            ScalarVisited = true;
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

        public bool AggregateVisited { private set; get; } = false;

        public bool BooleanVisited { private set; get; } = false;

        public bool CompositeVisited { private set; get; } = false;

        public bool ContainsVisited { private set; get; } = false;

        public bool FieldVisited { private set; get; } = false;

        public bool FieldDeclarationVisited { private set; get; } = false;

        public bool JoinVisited { private set; get; } = false;

        public bool LiteralVisited { private set; get; } = false;

        public bool NullVisited { private set; get; } = false;

        public bool ScalarVisited { private set; get; } = false;

        public bool SelectVisited { private set; get; } = false;

        public bool TableVisited { private set; get; } = false;

        public bool WhereVisited { private set; get; } = false;
    }
}

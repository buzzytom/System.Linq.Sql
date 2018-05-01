namespace LinqSql.Expressions.Tests
{
    public class MockExpressionVisitor : AExpressionVisitor
    {
        public override void VisitField(FieldExpression expression)
        {
            FieldVisited = true;
        }

        public override void VisitSelect(SelectExpression expression)
        {
            SelectVisited = true;
        }

        public override void VisitTable(TableExpression expression)
        {
            TableVisited = true;
        }

        // ----- Properties ----- //

        public bool FieldVisited { private set; get; } = false;

        public bool SelectVisited { private set; get; } = false;

        public bool TableVisited { private set; get; } = false;
    }
}

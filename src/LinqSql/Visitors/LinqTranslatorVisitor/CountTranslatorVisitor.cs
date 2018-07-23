using System.Linq.Expressions;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        private AggregateExpression VisitCount(MethodCallExpression expression)
        {
            Type type = expression.Method.DeclaringType;
            if (type == typeof(SqlQueryableHelper) || type == typeof(Enumerable) || type == typeof(Queryable))
            {
                // Map the source
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);

                // Resolve the optional predicate
                if (expression.Arguments.Count > 1)
                {
                    LambdaExpression lambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                    APredicateExpression predicate = Visit<APredicateExpression>(lambda.Body);
                    source = new WhereExpression(source, predicate);
                }

                // Resolve the field to be counted (must be done after the source has been manipulated)
                FieldExpression field = source.Fields.First();

                // Create the expression
                return new AggregateExpression(source, field, AggregateFunction.Count);
            }

            throw new MethodTranslationException(expression.Method);
        }
    }
}

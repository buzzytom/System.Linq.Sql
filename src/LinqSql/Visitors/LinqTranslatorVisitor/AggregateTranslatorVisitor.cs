using System.Linq.Expressions;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        private AggregateExpression VisitAggregate(MethodCallExpression expression, AggregateFunction function)
        {
            Type type = expression.Method.DeclaringType;
            if (type == typeof(SqlQueryableHelper) || type == typeof(Enumerable) || type == typeof(Queryable))
            {
                // Resolve the source
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);

                // Resolve the optional selector
                if (expression.Arguments.Count > 1)
                {
                    LambdaExpression lambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                    FieldExpression found = Visit<FieldExpression>(lambda.Body);
                    source = new SelectExpression(source, new[] { found });
                }

                // Resolve the field to be counted (must be done after the source has been manipulated)
                FieldExpression field = source.Fields.First();

                // Create the expression
                return new AggregateExpression(source, field, function);
            }

            throw new MethodTranslationException(expression.Method);
        }
    }
}

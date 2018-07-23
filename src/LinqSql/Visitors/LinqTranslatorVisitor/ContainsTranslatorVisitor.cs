using System.Linq.Expressions;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        private ContainsExpression VisitContains(MethodCallExpression expression)
        {
            // Handle extension methods defined by Linqs
            if (expression.Method.DeclaringType == typeof(Enumerable) || expression.Method.DeclaringType == typeof(Queryable))
            {
                AExpression values = Visit<AExpression>(expression.Arguments[0]);
                AExpression value = Visit<AExpression>(expression.Arguments[1]);
                return new ContainsExpression(values, value);
            }

            // Handle custom extension method
            if (expression.Method.DeclaringType == typeof(SqlQueryableHelper))
            {
                // Get value expression first, because the source will change to the subquery making the value out of scope
                AExpression value = Visit<AExpression>(expression.Arguments[2]);

                // Evaluate the subquery expressions
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);
                LambdaExpression fieldLambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                FieldExpression field = Visit<FieldExpression>(fieldLambda.Body);

                // Create the expression
                return new ContainsExpression(new ScalarExpression(source, field), value);
            }

            throw new MethodTranslationException(expression.Method);
        }
    }
}

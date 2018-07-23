using System.Linq.Expressions;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        private WhereExpression VisitWhere(MethodCallExpression expression)
        {
            // Handle the default Queryable extension Where
            if (expression.Method.DeclaringType == typeof(Queryable))
            {
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(expression.Arguments[1]);
                APredicateExpression predicate = Visit<APredicateExpression>(lambda.Body);
                return new WhereExpression(source, predicate);
            }

            throw new MethodTranslationException(expression.Method);
        }
    }
}

using System.Linq.Expressions;

namespace System.Linq.Sql
{
    public partial class LinqTranslatorVisitor
    {
        private WhereExpression VisitUpdate(MethodCallExpression expression)
        {
            if (IsDeclaring(expression, typeof(SqlQueryableHelper)))
            {
                ASourceExpression source = Visit<ASourceExpression>(expression.Arguments[0]);
                LiteralExpression table = Visit<LiteralExpression>(expression.Arguments[1]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(expression.Arguments[2]);

                throw new NotImplementedException();
            }

            throw new MethodTranslationException(expression.Method);
        }
    }
}

namespace System.Linq.Sql.Expressions
{
    using Queryable;

    /// <summary>
    /// <see cref="ASourceExpression"/> provides the base class from which concrete source expressions are derived. This is an abstract class. This class extends <see cref="AExpression"/>.
    /// </summary>
    public abstract class ASourceExpression : AExpression
    {
        /// <summary>Gets the field expressions exposed by this source.</summary>
        public virtual FieldExpressions Fields => null;

        /// <summary>Gets the static type of the expression that this System.Linq.Expressions.Expression represents.</summary>
        public override Type Type => typeof(IQueryable<Record>);
    }
}

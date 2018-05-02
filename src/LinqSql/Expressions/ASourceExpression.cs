using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqSql.Expressions
{
    /// <summary>
    /// <see cref="ASourceExpression"/> provides the base class from which concrete source expressions are derived. This is an abstract class. This class extends <see cref="AExpression"/>.
    /// </summary>
    public abstract class ASourceExpression : Expression
    {
        /// <summary>Gets the field expressions exposed by this source.</summary>
        public virtual IEnumerable<FieldExpression> Fields => null;
    }
}

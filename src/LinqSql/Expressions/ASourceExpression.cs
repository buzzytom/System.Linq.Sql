﻿using System.Collections.Generic;

namespace LinqSql.Expressions
{
    /// <summary>
    /// <see cref="ASourceExpression"/> provides the base class from which concrete source expressions are derived. This is an abstract class. This class extends <see cref="AExpression"/>.
    /// </summary>
    public abstract class ASourceExpression : AExpression
    {
        /// <summary>Gets the field expressions exposed by this source.</summary>
        public virtual IEnumerable<FieldExpression> Fields => null;
    }
}

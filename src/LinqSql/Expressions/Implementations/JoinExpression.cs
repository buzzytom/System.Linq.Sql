using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Sql
{
    /// <summary>
    /// An enumeration of the type of query joins that a <see cref="JoinExpression"/> can represent.
    /// </summary>
    public enum JoinType
    {
        /// <summary>Indicates that an inner and outer key must exist for a result to be generated to generated for a row.</summary>
        Inner,
        /// <summary>Indicates that all outer keys will be selected regardless of whether a matching inner key exists.</summary>
        Left,
        /// <summary>Indicates that all inner keys will be selected regardless of whether a matching outer key exists.</summary>
        Right
    }

    /// <summary>
    /// Represents the aggregation of two query sources.
    /// </summary>
    public class JoinExpression : ASourceExpression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="JoinExpression"/> with the specified sources.
        /// </summary>
        /// <param name="outer">The outer source expression to aggregate.</param>
        /// <param name="inner">The inner source expression to aggregate.</param>
        /// <param name="predicate">The optional predicate to condition the join on.</param>
        /// <param name="fields">The fields to select from the sources. If null, all fields are selected.</param>
        /// <param name="joinType">The type of join to perform.</param>
        public JoinExpression(ASourceExpression outer, ASourceExpression inner, APredicateExpression predicate = null, IEnumerable<FieldExpression> fields = null, JoinType joinType = JoinType.Inner)
        {
            if (outer == null)
                throw new ArgumentNullException(nameof(outer));
            if (inner == null)
                throw new ArgumentNullException(nameof(inner));

            Outer = outer;
            Inner = inner;
            Predicate = predicate ?? new BooleanExpression(true);
            JoinType = joinType;

            fields = fields ?? outer.Fields.Concat(inner.Fields);
            Fields = new FieldExpressions(this, fields);
            Expressions = new[] { outer, inner };
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected override Expression AcceptSql(IQueryExpressionVisitor visitor)
        {
            return visitor.VisitJoin(this);
        }

        // ----- Properties ----- //

        /// <summary>Gets the outer source of the expression.</summary>
        public ASourceExpression Outer { get; } = null;

        /// <summary>Gets the inner source of the expression.</summary>
        public ASourceExpression Inner { get; } = null;

        /// <summary>Gets the predicate of this epxression.</summary>
        public APredicateExpression Predicate { get; } = null;

        /// <summary>Gets the type of join this expression is.</summary>
        public JoinType JoinType { get; } = JoinType.Inner;

        /// <summary>Gets the given alias of the physical table that this table expression represents.</summary>
        public override FieldExpressions Fields { get; } = null;

        /// <summary>Gets the child expressions of this source.</summary>
        public override IEnumerable<ASourceExpression> Expressions { get; } = null;
    }
}

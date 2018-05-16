using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="SqlExpressionVisitor"/> is an implementation of <see cref="ISqlExpressionVisitor"/>, which the visit implementations generate an SQL representation of an expression tree.
    /// </summary>
    public class SqlExpressionVisitor : ExpressionVisitor, ISqlExpressionVisitor
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqlExpressionVisitor"/>.
        /// </summary>
        public SqlExpressionVisitor()
        { }

        /// <summary>
        /// Generates the sql representation of the specified expression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree to generate sql for.</param>
        /// <returns>The sql representation of the specified expression.</returns>
        /// <remarks>This method will clear any state currently executing on the visitor.</remarks>
        public Query GenerateQuery(SelectExpression expression)
        {
            Builder.Clear();
            Context.Clear();
            Builder.Append("select * from ");
            Visit(expression);
            return new Query(Builder.ToString(), Context.Parameters);
        }

        public override Expression Visit(Expression node)
        {
            if (node is ASourceExpression source)
                Context.Source = source;

            return base.Visit(node);
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitBoolean(BooleanExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (expression.Value)
                Builder.Append("true");
            else
                Builder.Append("false");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitComposite(CompositeExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Builder.Append("(");
            Visit(expression.Left);

            if (expression.Operator == CompositeOperator.And)
                Builder.Append(" and ");
            else if (expression.Operator == CompositeOperator.Or)
                Builder.Append(" or ");
            else if (expression.Operator == CompositeOperator.GreaterThan)
                Builder.Append(" > ");
            else if (expression.Operator == CompositeOperator.GreaterThanOrEqual)
                Builder.Append(" >= ");
            else if (expression.Operator == CompositeOperator.LessThan)
                Builder.Append(" < ");
            else if (expression.Operator == CompositeOperator.LessThanOrEqual)
                Builder.Append(" <= ");
            else if (expression.Operator == CompositeOperator.Equal)
            {
                if (expression.Right is NullExpression)
                    Builder.Append(" is ");
                else
                    Builder.Append(" = ");
            }
            else if (expression.Operator == CompositeOperator.NotEqual)
            {
                if (expression.Right is NullExpression)
                    Builder.Append(" is not ");
                else
                    Builder.Append(" <> ");
            }
            else
                throw new NotSupportedException($"Cannot generate sql for '{expression.Operator}' operator of {nameof(CompositeExpression)}.");

            Visit(expression.Right);
            Builder.Append(")");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitField(FieldExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (Context.Source == null)
                throw new InvalidOperationException($"A field cannot be visited unless an {nameof(ASourceExpression)} has been visited.");

            string table = Context.GetSource(expression.Expression);
            string field = Context.Source.Fields.GetKey(expression);
            Builder.Append($"[{table}].[{field}]");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitJoin(JoinExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            // Build the outer selector
            Builder.Append("(select ");
            VisitFields(expression, expression.Fields);
            Builder.Append(" from ");
            Visit(expression.Outer);

            // Add the join keyword
            switch (expression.JoinType)
            {
                case JoinType.Inner:
                    Builder.Append("join");
                    break;
                case JoinType.Left:
                    Builder.Append("left join");
                    break;
                case JoinType.Right:
                    Builder.Append("right join");
                    break;
                default:
                    throw new NotSupportedException($"The {nameof(JoinType)} {expression.JoinType.ToString()} is not supported by {nameof(SqlExpressionVisitor)}.");
            }

            // Build the inner selector
            Visit(expression.Inner);

            // Build the predicate
            Builder.Append("on");
            Visit(expression.Predicate);

            // Finalize the join
            Builder.Append($") as [{Context.GetSource(expression)}]");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitLiteral(LiteralExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            string key = Context.CreateParameter(expression.Value);
            Builder.Append($"@{key}");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitNull(NullExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Builder.Append("null");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitSelect(SelectExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Builder.Append("(select ");
            VisitFields(expression, expression.Fields);
            Builder.Append(" from ");
            Visit(expression.Source);
            Builder.Append($") as [{Context.GetSource(expression)}]");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitTable(TableExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Builder.Append("(select ");
            VisitFields(expression, expression.Fields);
            Builder.Append($" from [{expression.Table}]) as [{Context.GetSource(expression)}]");

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitWhere(WhereExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Builder.Append("(select * from ");
            Visit(expression.Source);
            Builder.Append(" where ");
            Visit(expression.Predicate);
            Builder.Append($") as [{Context.GetSource(expression)}]");

            return expression;
        }

        /// <summary>
        /// Visits the specified expressions.
        /// </summary>
        /// <param name="expression">The expression the fields are being rendered for.</param>
        /// <param name="fields">A collection of expressions to visit.</param>
        protected virtual void VisitFields(ASourceExpression expression, IEnumerable<FieldExpression> fields)
        {
            StringBuilder builder = new StringBuilder();
            foreach (FieldExpression field in fields)
            {
                // Punctuation
                if (builder.Length > 0)
                    builder.Append(",");

                // Render the table
                if (field.Source != null)
                    builder.Append($"[{Context.GetSource(field.Source.Expression)}].");

                // Decode the field name
                string name = field.Source?.Expression.Fields.GetKey(field.Source) ?? field.FieldName;

                // Render the field
                builder.Append($"[{name}]as[{expression.Fields.GetKey(field)}]");
            }
            Builder.Append(builder.ToString());
        }

        // ----- Properties ----- //

        /// <summary>Gets the string builder used to form the query.</summary>
        protected StringBuilder Builder { get; } = new StringBuilder();

        /// <summary>Gets the vistor context used during the visitation of expressions.</summary>
        protected SqlVisitorContext Context { get; } = new SqlVisitorContext();

        /// <summary>Gets the current sql state of the visitor.</summary>
        public string SqlState => Builder.ToString();
    }
}

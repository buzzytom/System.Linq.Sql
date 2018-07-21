using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace System.Linq.Sql
{
    /// <summary>
    /// <see cref="QueryExpressionVisitor"/> is an implementation of <see cref="IQueryExpressionVisitor"/>, which the visit implementations generate an SQL representation of an expression tree.
    /// </summary>
    public class QueryExpressionVisitor : ExpressionVisitor, IQueryExpressionVisitor
    {
        /// <summary>
        /// Creates a new instance of <see cref="QueryExpressionVisitor"/>.
        /// </summary>
        public QueryExpressionVisitor()
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
            Builder.Append("select * from (");
            Visit(expression);
            Builder.Append(")");
            return new Query(Builder.ToString(), Context.Parameters);
        }

        /// <summary>
        /// Dispatches the list of expressions to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">The expressions to visit.</param>
        /// <returns>The modified expression list, if any one of the elements were modified; otherwise, returns the original expression list.</returns>
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
        public virtual Expression VisitAggregate(AggregateExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Builder.Append(GetAggregateFunction(expression.Function));
            Builder.Append("(");
            Visit(expression.SourceField);
            Builder.Append(")");

            return expression;
        }

        /// <summary>
        /// Gets the sql name of the specified <see cref="AggregateFunction"/>.
        /// </summary>
        /// <param name="function">The function to get the sql name of.</param>
        /// <returns>The sql name of the specified aggregate function.</returns>
        /// <exception cref="NotSupportedException">Thrown if the specified function is not a known function by the expression visitor.</exception>
        protected virtual string GetAggregateFunction(AggregateFunction function)
        {
            switch (function)
            {
                case AggregateFunction.Average:
                    return "avg";
                case AggregateFunction.Count:
                    return "count";
                case AggregateFunction.Max:
                    return "max";
                case AggregateFunction.Min:
                    return "min";
                case AggregateFunction.Sum:
                    return "sum";
                default:
                    throw new NotSupportedException($"The aggregate function {function.ToString()} is not known by the sql expression visitor.");
            }
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
        public virtual Expression VisitContains(ContainsExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Visit(expression.Value);
            Builder.Append(" in (");
            Visit(expression.Values);
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

            if (!(expression.ValueExpression is ASourceExpression sourceValueExpression))
                throw new InvalidOperationException($"A field exposed from a source expression must itself have an {nameof(ASourceExpression)}. Did you mean to call VisitFieldDeclaration?");
            else
            {
                // Get the source associated with field
                ASourceExpression source = Context.Source.Expressions.First(x => x.Fields.Contains(expression));

                // Render the field
                string table = Context.GetSource(sourceValueExpression);
                string field = source.Fields.GetKey(expression);
                Builder.Append($"[{table}].[{field}]");
            }

            return expression;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        public virtual Expression VisitFieldDeclaration(FieldExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (Context.Source == null)
                throw new InvalidOperationException($"A field cannot be visited unless an {nameof(ASourceExpression)} has been visited.");

            // Resolve the fields alias
            // Note: Must be calculated before the field is rendered because child visitation will change the Context.Source value.
            string alias = Context.Source.Fields.GetKey(expression);

            // Render the field source value, detecting if the source is a referencable source
            ASourceExpression sourceExpression = expression.SourceExpression?.ValueExpression as ASourceExpression;
            if (expression.SourceExpression != null && sourceExpression == null)
                Visit(expression.SourceExpression.ValueExpression);
            else
            {
                // Render the table
                if (sourceExpression != null)
                    Builder.Append($"[{Context.GetSource(sourceExpression)}].");

                // Decode the field name
                string name = sourceExpression?.Fields.GetKey(expression.SourceExpression) ?? expression.FieldName;
                Builder.Append($"[{name}]");
            }

            // Render the field
            Builder.Append($" as [{alias}]");

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
                    throw new NotSupportedException($"The {nameof(JoinType)} {expression.JoinType.ToString()} is not supported by {nameof(QueryExpressionVisitor)}.");
            }

            // Build the inner selector
            Visit(expression.Inner);

            // Build the predicate
            Builder.Append("on");
            Context.Source = expression;
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

            if (!(expression.Value is Array collection))
                Builder.Append($"@{Context.CreateParameter(expression.Value)}");
            else
            {
                for (int i = 0; i < collection.Length; i++)
                {
                    if (i > 0)
                        Builder.Append(", ");
                    Builder.Append($"@{Context.CreateParameter(collection.GetValue(i))}");
                }
            }

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
        public virtual Expression VisitScalar(ScalarExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Builder.Append("select ");
            VisitFields(expression, expression.Fields);
            if (expression.Source != null)
            {
                Builder.Append(" from ");
                Visit(expression.Source);
            }

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

            // Render the body declaration
            Builder.Append("(select ");
            VisitFields(expression, expression.Fields);

            // Render the source expression
            if (expression.Source != null)
            {
                Builder.Append(" from ");
                Visit(expression.Source);
            }

            // Restore the current source
            Context.Source = expression;

            // Render the orderings
            if (expression.Orderings != null && expression.Orderings.Any())
                RenderOrderings(expression.Orderings);

            // Render the range selection
            if (expression.Skip > 0 || expression.Take >= 0)
                RenderLimit(expression.Skip < 0 ? 0 : expression.Skip, expression.Take < 0 ? long.MaxValue : expression.Take);

            // Render the expression alias
            Builder.Append($") as [{Context.GetSource(expression)}]");

            return expression;
        }

        /// <summary>
        /// Renders the specified limit options.
        /// </summary>
        /// <param name="skip">The number of result rows to skip before reading.</param>
        /// <param name="take">The number of result rows to read.</param>
        protected virtual void RenderLimit(long skip, long take)
        {
            Builder.Append(" offset ");
            Builder.Append(skip);
            Builder.Append(" rows fetch next ");
            Builder.Append(take);
            Builder.Append(" rows only");
        }

        /// <summary>
        /// Renders the specified orderings collection.
        /// </summary>
        /// <param name="orderings">The orderings to render.</param>
        protected virtual void RenderOrderings(IEnumerable<Ordering> orderings)
        {
            Builder.Append(" order by ");
            bool first = true;
            foreach (Ordering ordering in orderings)
            {
                // Render punctuation
                if (!first)
                    Builder.Append(", ");
                first = false;

                // Render column
                VisitField(ordering.Field);

                // Render direction
                if (ordering.OrderType == OrderType.Ascending)
                    Builder.Append(" asc");
                else
                    Builder.Append(" desc");
            }
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
            Context.Source = expression;
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
            Context.Source = expression;
            bool first = true;
            foreach (FieldExpression field in fields)
            {
                // Punctuation
                if (!first)
                    Builder.Append(",");
                first = false;

                // Render the declaration
                // Note: Have to use accept because Visit is protected
                field.AcceptDeclarationSql(this);
            }
        }

        // ----- Properties ----- //

        /// <summary>Gets the string builder used to form the query.</summary>
        protected StringBuilder Builder { get; } = new StringBuilder();

        /// <summary>Gets the vistor context used during the visitation of expressions.</summary>
        protected QueryVisitorContext Context { get; } = new QueryVisitorContext();

        /// <summary>Gets the current sql state of the visitor.</summary>
        public string SqlState => Builder.ToString();
    }
}

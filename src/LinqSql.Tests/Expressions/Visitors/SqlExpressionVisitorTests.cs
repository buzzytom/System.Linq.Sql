using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class SqlExpressionVisitorTests
    {
        private readonly SqlExpressionVisitor visitor = new SqlExpressionVisitor();

        [TestMethod]
        public void SqlExpressionVisitor_GenerateSql()
        {
            // Prepare the test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            TableExpression table = new TableExpression("Table", "Alias", fields);
            SelectExpression expression = new SelectExpression(table);

            // Performs the test operation
            Query query = visitor.GenerateQuery(expression);

            // Check the result
            Assert.AreEqual("select * from (select [t0].[f0]as[f0],[t0].[f1]as[f1] from (select [FieldA]as[f0],[FieldB]as[f1] from [Table]) as [t0]) as [t1]", query.Sql);
        }

        [TestMethod]
        public void SqlExpressionVisitor_Visit_ArgumentNullExceptions()
        {
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitBoolean(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitComposite(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitContains(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitField(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitJoin(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitLiteral(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitNull(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitSelect(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitTable(null));
            Assert.ThrowsException<ArgumentNullException>(() => visitor.VisitWhere(null));
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitBoolean()
        {
            // Prepare the test data
            BooleanExpression a = new BooleanExpression(true);
            BooleanExpression b = new BooleanExpression(false);

            // Perform the test operation
            visitor.VisitBoolean(a);
            visitor.VisitBoolean(b);

            // Check the test result
            Assert.AreEqual("truefalse", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite()
        {
            // Prepare the test data
            BooleanExpression a = new BooleanExpression(true);
            BooleanExpression b = new BooleanExpression(false);
            CompositeExpression expression = new CompositeExpression(a, new CompositeExpression(a, b, CompositeOperator.Or), CompositeOperator.And);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(true and (true or false))", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitContains()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite_GreaterThan()
        {
            // Prepare the test data
            CompositeExpression expression = new CompositeExpression(new LiteralExpression(1), new LiteralExpression(2), CompositeOperator.GreaterThan);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(@p0 > @p1)", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite_GreaterThanOrEqual()
        {
            // Prepare the test data
            CompositeExpression expression = new CompositeExpression(new LiteralExpression(1), new LiteralExpression(2), CompositeOperator.GreaterThanOrEqual);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(@p0 >= @p1)", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite_LessThan()
        {
            // Prepare the test data
            CompositeExpression expression = new CompositeExpression(new LiteralExpression(1), new LiteralExpression(2), CompositeOperator.LessThan);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(@p0 < @p1)", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite_LessThanOrEqual()
        {
            // Prepare the test data
            CompositeExpression expression = new CompositeExpression(new LiteralExpression(1), new LiteralExpression(2), CompositeOperator.LessThanOrEqual);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(@p0 <= @p1)", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite_Equal()
        {
            // Prepare the test data
            CompositeExpression expression = new CompositeExpression(new LiteralExpression(1), new LiteralExpression(2), CompositeOperator.Equal);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(@p0 = @p1)", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite_Equal_Null()
        {
            // Prepare the test data
            CompositeExpression expression = new CompositeExpression(new LiteralExpression(1), new NullExpression(), CompositeOperator.Equal);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(@p0 is null)", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite_NotEqual()
        {
            // Prepare the test data
            CompositeExpression expression = new CompositeExpression(new LiteralExpression(1), new LiteralExpression(2), CompositeOperator.NotEqual);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(@p0 <> @p1)", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitComposite_NotEqual_Null()
        {
            // Prepare the test data
            CompositeExpression expression = new CompositeExpression(new LiteralExpression(1), new NullExpression(), CompositeOperator.NotEqual);

            // Perform the test operation
            visitor.VisitComposite(expression);

            // Check the test result
            Assert.AreEqual("(@p0 is not null)", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitField()
        {
            // Prepare the test data
            TableExpression table = new TableExpression("Table", "Alias", new string[] { "Field" });
            APredicateExpression predicate = new CompositeExpression(table.Fields.First(), new NullExpression(), CompositeOperator.Equal);
            WhereExpression expression = new WhereExpression(table, predicate);

            // Perform the test operation
            visitor.VisitWhere(expression);

            // Check the test result
            // Note: The important part of this check is the "[t0].[f0]"
            Assert.AreEqual("(select * from (select [Field]as[f0] from [Table]) as [t0] where ([t0].[f0] is null)) as [t1]", visitor.SqlState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SqlExpressionVisitor_VisitField_InvalidOperationException()
        {
            // Prepare the test data
            SelectExpression source = new SelectExpression(new TableExpression("Table", "Alias", new string[] { "Field" }));
            FieldExpression expression = source.Fields.FirstOrDefault();

            // Perform the test operation
            visitor.VisitField(expression);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitJoin()
        {
            // Prepare the test data
            TableExpression outer = new TableExpression("Outer", "OuterAlias", new string[] { "OuterField" });
            TableExpression inner = new TableExpression("Inner", "InnerAlias", new string[] { "InnerField" });
            APredicateExpression predicate = new CompositeExpression(outer.Fields.First(), inner.Fields.First(), CompositeOperator.Equal);
            JoinExpression expression = new JoinExpression(outer, inner, predicate, null, JoinType.Left);

            // Perform the test operation
            visitor.VisitJoin(expression);

            // Check the test result
            Assert.AreEqual("(select [t0].[f0]as[f0],[t1].[f0]as[f1] from (select [OuterField]as[f0] from [Outer]) as [t0]left join(select [InnerField]as[f0] from [Inner]) as [t1]on([t0].[f0] = [t1].[f0])) as [t2]", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitLiteral()
        {
            // Prepare the test data
            LiteralExpression a = new LiteralExpression("Hello World!");
            LiteralExpression b = new LiteralExpression("Hello World!");
            LiteralExpression c = new LiteralExpression("Another World!");

            // Perform the test operation
            visitor.VisitLiteral(a);
            visitor.VisitLiteral(a);
            visitor.VisitLiteral(b);
            visitor.VisitLiteral(b);
            visitor.VisitLiteral(c);

            // Check the test result
            Assert.AreEqual("@p0@p0@p0@p0@p1", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitNull()
        {
            // Prepare the test data
            NullExpression expression = new NullExpression();

            // Perform the test operation
            visitor.VisitNull(expression);

            // Check the result
            Assert.AreEqual("null", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitSelect()
        {
            // Prepare the test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            TableExpression table = new TableExpression("Table", "Alias", fields);
            SelectExpression expression = new SelectExpression(table);

            // Performs the test operation
            visitor.VisitSelect(expression);

            // Check the result
            Assert.AreEqual("(select [t0].[f0]as[f0],[t0].[f1]as[f1] from (select [FieldA]as[f0],[FieldB]as[f1] from [Table]) as [t0]) as [t1]", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitTable()
        {
            // Prepare the test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            TableExpression expression = new TableExpression("Table", "Alias", fields);

            // Perform the test operation
            visitor.VisitTable(expression);

            // Check the result
            Assert.AreEqual("(select [FieldA]as[f0],[FieldB]as[f1] from [Table]) as [t0]", visitor.SqlState);
        }

        [TestMethod]
        public void SqlExpressionVisitor_VisitWhere()
        {
            // Prepare the test data
            string[] fields = new string[] { "FieldA", "FieldB" };
            WhereExpression expression = new WhereExpression(new TableExpression("Table", "Alias", fields), new BooleanExpression(true));

            // Perform the test operation
            visitor.VisitWhere(expression);

            // Check the result
            Assert.AreEqual("(select * from (select [FieldA]as[f0],[FieldB]as[f1] from [Table]) as [t0] where true) as [t1]", visitor.SqlState);
        }
    }
}

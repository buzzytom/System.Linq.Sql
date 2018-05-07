﻿using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class FieldExpressionTests
    {
        private FieldExpression expression = new FieldExpression("TableName", "FieldName");

        [TestMethod]
        public void FieldExpression_Constructor_Exceptions()
        {
            Assert.ThrowsException<ArgumentException>(() => new FieldExpression("", "FieldName"));
            Assert.ThrowsException<ArgumentException>(() => new FieldExpression("TableName", ""));
        }

        [TestMethod]
        public void FieldExpression_Properties()
        {
            Assert.AreEqual(ExpressionType.Extension, expression.NodeType);
            Assert.AreEqual(typeof(object), expression.Type);
            Assert.AreEqual("TableName", expression.TableName);
            Assert.AreEqual("FieldName", expression.FieldName);
        }

        [TestMethod]
        public void FieldExpression_Accept()
        {
            // Setup test
            MockExpressionVisitor visitor = new MockExpressionVisitor();

            // Perform the test operation
            visitor.Visit(expression);

            // Check test result
            Assert.IsTrue(visitor.FieldVisited);
        }
    }
}

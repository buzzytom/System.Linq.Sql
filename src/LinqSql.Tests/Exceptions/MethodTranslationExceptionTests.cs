using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq.Sql.Tests
{
    [TestClass]
    public class MethodTranslationExceptionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MethodTranslationException_Constructor_ArgumentNullException()
        {
            new MethodTranslationException(null);
        }
    }
}

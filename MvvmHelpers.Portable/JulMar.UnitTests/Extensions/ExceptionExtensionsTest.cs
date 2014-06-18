using System;
using JulMar.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JulMar.Tests.Extensions
{
    /// <summary>
    ///This is a test class for ExceptionExtensionsTest and is intended
    ///to contain all ExceptionExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExceptionExtensionsTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for Flatten
        ///</summary>
        [TestMethod()]
        public void FlattenTest()
        {
            try
            {
                GenerateException(5, 0);
            }
            catch (Exception ex)
            {
                string test = ex.Flatten("Outer Exception: ");
                Assert.Inconclusive(test);
            }
        }

        private static int GenerateException(int x, int y)
        {
            try
            {
                return x / y;
            }
            catch (Exception ex)
            {
                string test = ex.Flatten("Inner Exception: ", true);
                throw new ApplicationException("Problem in application", ex);
            }
        }
    }
}

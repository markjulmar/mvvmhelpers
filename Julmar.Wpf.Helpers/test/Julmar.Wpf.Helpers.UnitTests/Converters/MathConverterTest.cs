using JulMar.Windows.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace Julmar.Wpf.Helpers.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for MathConverterTest and is intended
    ///to contain all MathConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MathConverterTest
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for Convert
        ///</summary>
        [TestMethod()]
        public void ConvertTest()
        {
            MathConverter cvt = new MathConverter();
            Assert.AreEqual("11", cvt.Convert(10, null, "+1", CultureInfo.CurrentCulture));
            Assert.AreEqual("11.75", cvt.Convert(10.25, null, "+1.5", CultureInfo.CurrentCulture));
            Assert.AreEqual("12", cvt.Convert("10.5", null, "+1.5", CultureInfo.CurrentCulture));
            Assert.AreEqual("8", cvt.Convert("10", null, "-2", CultureInfo.CurrentCulture));
            Assert.AreEqual("3", cvt.Convert((float)1, null, "+2", CultureInfo.CurrentCulture));
            Assert.AreEqual("-2", cvt.Convert(-4L, null, "+2", CultureInfo.CurrentCulture));
            Assert.AreEqual("16", cvt.Convert("4", null, "*4", CultureInfo.CurrentCulture));
            Assert.AreEqual("5", cvt.Convert("10", null, "/2", CultureInfo.CurrentCulture));
            Assert.AreEqual("0", cvt.Convert(10, null, "%2", CultureInfo.CurrentCulture));
        }
    }
}

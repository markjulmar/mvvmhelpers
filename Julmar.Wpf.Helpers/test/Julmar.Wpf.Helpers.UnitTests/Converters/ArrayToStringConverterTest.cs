using JulMar.Windows.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Julmar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for ArrayToStringConverterTest and is intended
    ///to contain all ArrayToStringConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArrayToStringConverterTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        [TestMethod()]
        public void NullTest()
        {
            ArrayToStringConverter target = new ArrayToStringConverter();
            object value = target.Convert(null, typeof (string), null, null);
            Assert.AreSame(string.Empty, value);
            value = target.ConvertBack(null, typeof(string), null, null);
            Assert.IsInstanceOfType(value, typeof(string[]));
        }

        [TestMethod]
        public void EmptyTest()
        {
            ArrayToStringConverter target = new ArrayToStringConverter();
            string value = (string) target.Convert(new [] { null, "", "", null}, typeof(string), null, null);
            Assert.AreEqual(value, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertBadParameters()
        {
            ArrayToStringConverter target = new ArrayToStringConverter();
            target.Convert("test", typeof (string), null, null);
        }

        [TestMethod]
        public void ConvertTest()
        {
            ArrayToStringConverter target = new ArrayToStringConverter();
            string val = (string) target.Convert(new[] {"Hello", "World."}, typeof (string), null, null);
            Assert.AreEqual("Hello,World.",val);
        }

        [TestMethod]
        public void ConvertTestWithBlanks()
        {
            ArrayToStringConverter target = new ArrayToStringConverter();
            string val = (string)target.Convert(new[] { "Hello", "", null, "World." }, typeof(string), null, null);
            Assert.AreEqual("Hello,World.", val);
        }

        [TestMethod]
        public void ConvertTestWithSeparator()
        {
            ArrayToStringConverter target = new ArrayToStringConverter() {Separator = " "};
            string val = (string)target.Convert(new[] { "This","is","a","Test" }, typeof(string), null, null);
            Assert.AreEqual("This is a Test", val);
        }

        [TestMethod]
        public void ConvertBackTest()
        {
            ArrayToStringConverter target = new ArrayToStringConverter();
            string[] expected = new[] {"Hello", "World."};
            string[] val = (string[])target.ConvertBack("Hello,World.", typeof(string[]), null, null);
            CollectionAssert.AreEqual(expected,val);
        }

        [TestMethod]
        public void ConvertBackBadParameters()
        {
            ArrayToStringConverter target = new ArrayToStringConverter() { Separator = "."};
            string[] value = (string[]) target.ConvertBack(Math.PI, typeof(string[]), null, null);
            Assert.AreEqual(2,value.Length);
            Assert.AreEqual("3",value[0]);
        }

        [TestMethod]
        public void ConvertBackTestWithSeparator()
        {
            ArrayToStringConverter target = new ArrayToStringConverter() { Separator = " " };
            string[] expected = new[] {"This", "is", "a", "Test"};
            string[] val = (string[])target.ConvertBack("This is a Test", typeof(string[]), null, null);
            CollectionAssert.AreEqual(expected,val);
        }

        class NullServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType) { return null; }
        }

        /// <summary>
        ///A test for ProvideValue
        ///</summary>
        [TestMethod()]
        public void ProvideValueTest()
        {
            object target = new ArrayToStringConverter().ProvideValue(new NullServiceProvider());
            Assert.IsInstanceOfType(target, typeof(ArrayToStringConverter));
        }
    }
}

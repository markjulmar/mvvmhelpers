using JulMar.Windows.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.ObjectModel;
using System;

namespace Julmar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for CollectionExtensionsTest and is intended
    ///to contain all CollectionExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CollectionExtensionsTest
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
        public void ForEachTest()
        {
            int sum = 0;
            var numbers = Enumerable.Range(1, 10);
            numbers.ForEach(i => sum += i);
            Assert.AreEqual(numbers.Sum(),sum);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ForEachTestNopredicte()
        {
            Enumerable.Range(1, 10).ForEach(null);
        }

        /// <summary>
        ///A test for AddRange
        ///</summary>
        [TestMethod]
        public void AddRangeTest()
        {
            ObservableCollection<int> coll = new ObservableCollection<int>(Enumerable.Range(1,10));
            coll.AddRange(Enumerable.Range(11,10));
            CollectionAssert.AreEqual(coll, Enumerable.Range(1, 20).ToList());
        }

        [TestMethod()]
        public void IndexOfTest()
        {
            int[] vals = Enumerable.Range(1, 10).ToArray();
            int index  = vals.IndexOf(i => i > 5);
            Assert.AreEqual(5,index);
        }

        [TestMethod()]
        public void IndexOfTestMissing()
        {
            int[] vals = Enumerable.Range(1, 10).ToArray();
            int index = vals.IndexOf(i => i > 20);
            Assert.AreEqual(-1, index);
        }
    }
}

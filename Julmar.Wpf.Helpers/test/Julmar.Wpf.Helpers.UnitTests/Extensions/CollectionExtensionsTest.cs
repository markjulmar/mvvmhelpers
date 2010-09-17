using JulMar.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.ObjectModel;
using System;

namespace JulMar.Wpf.Helpers.UnitTests
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CompareNull()
        {
            int[] coll1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Assert.IsFalse(coll1.Compare(null));
        }

        [TestMethod]
        public void CompareEqual()
        {
            int[] coll1 = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            int[] coll2 = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

            Assert.IsTrue(coll1.Compare(coll2));
            Assert.IsTrue(coll1.Compare(coll2, true));
        }

        [TestMethod]
        public void CompareEqual2()
        {
            int[] coll1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[] coll2 = {9, 8, 7, 6, 5, 4, 3, 2, 1, 0};

            Assert.IsTrue(coll1.Compare(coll2));
            Assert.IsFalse(coll1.Compare(coll2, true));
        }

        [TestMethod]
        public void CompareMismatchCount()
        {
            int[] coll1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[] coll2 = { 0, 1, 2, 3, 5, 6, 7, 8, 9 };

            Assert.IsFalse(coll1.Compare(coll2));
            Assert.IsFalse(coll1.Compare(coll2, true));
        }

        [TestMethod]
        public void CompareUnordered()
        {
            int[] coll1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[] coll2 = {3, 5, 7, 9, 1, 2, 4, 6, 8, 0};

            Assert.IsTrue(coll1.Compare(coll2));
            Assert.IsFalse(coll1.Compare(coll2, true));
        }

        class TestObj{}

        [TestMethod]
        public void CompareRefTypes()
        {
            var coll1 = new[] {new TestObj(), new TestObj(), new TestObj()};
            var coll2 = new[] { new TestObj(), new TestObj(), new TestObj() };

            Assert.IsFalse(coll1.Compare(coll2));
            Assert.IsFalse(coll1.Compare(coll2, true));
        }

        class TestObj2 : IEquatable<TestObj2>
        {
            private readonly int val;
            public TestObj2(int val)
            {
                this.val = val;
            }

            public override bool Equals(object obj)
            {
                return obj is TestObj2 ? Equals((TestObj2) obj) : base.Equals(obj);
            }

            public bool Equals(TestObj2 other)
            {
                return other != null && (ReferenceEquals(this, other) || other.val == val);
            }

            public override int GetHashCode()
            {
                return val;
            }
        }

        [TestMethod]
        public void CompareRefTypesWithEquality()
        {
            var coll1 = new[] { new TestObj2(1), new TestObj2(2), new TestObj2(3) };
            var coll2 = new[] { new TestObj2(1), new TestObj2(2), new TestObj2(3) };

            Assert.IsTrue(coll1.Compare(coll2));
            Assert.IsTrue(coll1.Compare(coll2, true));
        }

        [TestMethod]
        public void CompareSameColl()
        {
            var coll1 = new[] { new TestObj(), new TestObj(), new TestObj() };

            Assert.IsTrue(coll1.Compare(coll1));
            Assert.IsTrue(coll1.Compare(coll1, true));
        }
    }
}

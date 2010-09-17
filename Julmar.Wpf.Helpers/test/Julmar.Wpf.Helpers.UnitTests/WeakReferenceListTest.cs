using System;
using JulMar.Core.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for WeakReferenceListTest and is intended
    ///to contain all WeakReferenceListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WeakReferenceListTest
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
        public void AddTest()
        {
            object[] data = {new object(), new object(), new object()};

            var wrl = new WeakReferenceList<object>() ;
            Assert.AreEqual(0, wrl.Count);
            foreach (var item in data)
                wrl.Add(item);
            Assert.AreEqual(3, wrl.Count);

            GC.Collect(2, GCCollectionMode.Forced);
            Assert.AreEqual(3, wrl.Count);
        }

        [TestMethod()]
        public void ClearTest()
        {
            object[] data = { new object(), new object(), new object() };
            var wrl = new WeakReferenceList<object>(data);
            Assert.AreEqual(3, wrl.Count);
            wrl.Clear();
            Assert.AreEqual(0, wrl.Count);
        }

        [TestMethod()]
        public void ContainsTest()
        {
            string[] testArray = {"1", "2", "3"};
            var wrl = new WeakReferenceList<string>(testArray);

            Assert.IsTrue(wrl.Contains("1"));
            Assert.IsTrue(wrl.Contains("2"));
            Assert.IsTrue(wrl.Contains("3"));
            Assert.IsFalse(wrl.Contains("4"));
        }

        [TestMethod()]
        public void CopyToTest()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);

            string[] testArray2 = new string[wrl.Count];
            wrl.CopyTo(testArray2, 0);
            CollectionAssert.AreEqual(testArray, testArray2);
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);

            int pos = 0;
            IEnumerator<string> ie = wrl.GetEnumerator();
            while (ie.MoveNext())
            {
                Assert.AreSame(testArray[pos++], ie.Current);
            }
        }

        [TestMethod()]
        public void GetItemTest()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);

            Assert.AreSame(wrl[2], testArray[2]);
        }

        [TestMethod()]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetItemTest_OutofRange()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);
            var item = wrl[10];
        }

        [TestMethod()]
        public void IndexOfTest()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);

            Assert.AreEqual(0, wrl.IndexOf("1"));
            Assert.AreEqual(1, wrl.IndexOf("2"));
            Assert.AreEqual(2, wrl.IndexOf("3"));
            Assert.AreEqual(-1, wrl.IndexOf("4"));
            Assert.AreEqual(-1, wrl.IndexOf(null));
        }

        [TestMethod()]
        public void InsertTest()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);
            wrl.Insert(1, "1.5");
            Assert.AreEqual(4, wrl.Count);
            Assert.AreEqual("2", wrl[2]);
            Assert.AreEqual("1.5", wrl[1]);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);
            wrl.Remove("2");
            Assert.AreEqual(2, wrl.Count);
            Assert.AreEqual("3", wrl[1]);
        }

        [TestMethod()]
        public void RemoveAtTest()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);
            wrl.RemoveAt(1);
            Assert.AreEqual(2, wrl.Count);
            Assert.AreEqual("3", wrl[1]);
        }

        [TestMethod()]
        public void ToListTest()
        {
            string[] testArray = { "1", "2", "3" };
            var wrl = new WeakReferenceList<string>(testArray);
            var newList = wrl.ToList();

            Assert.AreEqual(3, newList.Count);
            Assert.AreEqual("1", newList[0]);
            Assert.AreEqual("2", newList[1]);
            Assert.AreEqual("3", newList[2]);
        }

        [TestMethod()]
        public void ToListCollectTest()
        {
            object[] testArray = { new object(), new object(), new object() };
            var wrl = new WeakReferenceList<object>(testArray);

            testArray = null;
            GC.Collect(2,GCCollectionMode.Forced);
            wrl.Add(new object());
            GC.Collect(2, GCCollectionMode.Forced);
            var newList = wrl.ToList();
            Assert.AreEqual(0, newList.Count);
        }

        [TestMethod()]
        public void CollectTest2()
        {
            object[] testArray = { new object(), new object(), new object() };
            var wrl = new WeakReferenceList<object>(testArray);

            testArray[1] = null;
            GC.Collect(2, GCCollectionMode.Forced);

            var newList = wrl.ToList();
            Assert.AreEqual(2, newList.Count);
            Assert.AreEqual(testArray[0], newList[0]);
            Assert.AreEqual(testArray[2], newList[1]);
        }

    }
}

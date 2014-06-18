using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;

using JulMar.Collections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JulMar.Tests.Core
{
    /// <summary>
    ///This is a test class for ObservableDictionaryTest and is intended
    ///to contain all ObservableDictionaryTest Unit Tests.  It only tests
    /// the new features -- not the entire Dictionary semantics.
    ///</summary>
    [TestClass()]
    public class ObservableDictionaryTest
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
            var target = new ObservableDictionary<int, string>();

            bool hitChange = false;
            const int key = 10;
            const string value = "Hello";

            target.CollectionChanged += (s, e) =>
            {
                hitChange = true;
                Assert.AreSame(target, s);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
                var item = (KeyValuePair<int, string>) e.NewItems[0];
                Assert.AreEqual(key, item.Key);
                Assert.AreEqual(value, item.Value);
            };

            target[key] = value;
            Assert.IsTrue(hitChange);
        }

        [TestMethod()]
        public void ReplaceTest()
        {
            var target = new ObservableDictionary<int, string>();

            bool hitChange = false;
            const int key = 10;
            const string value = "Hello";
            const string value2 = "World";

            target[key] = value;

            target.CollectionChanged += (s, e) =>
            {
                hitChange = true;
                Assert.AreSame(target, s);
                Assert.AreEqual(NotifyCollectionChangedAction.Replace, e.Action);
                var oldItem = (KeyValuePair<int, string>)e.OldItems[0];
                Assert.AreEqual(key, oldItem.Key);
                Assert.AreEqual(value, oldItem.Value);
                var newItem = (KeyValuePair<int, string>)e.NewItems[0];
                Assert.AreEqual(key, newItem.Key);
                Assert.AreEqual(value2, newItem.Value);
            };

            target[key] = value2;
            Assert.IsTrue(hitChange);
        }

        [TestMethod()]
        public void ClearTest()
        {
            var target = new ObservableDictionary<int, string>();

            bool hitChange = false;
            const int key = 10;
            const string value = "Hello";

            target[key] = value;

            target.CollectionChanged += (s, e) =>
            {
                hitChange = true;
                Assert.AreSame(target, s);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, e.Action);
            };

            target.Clear();

            Assert.IsTrue(hitChange);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            var target = new ObservableDictionary<int, string>();

            bool hitChange = false;
            const int key = 10;
            const string value = "Hello";

            target[key] = value;

            target.CollectionChanged += (s, e) =>
            {
                hitChange = true;
                Assert.AreSame(target, s);
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
                var oldItem = (KeyValuePair<int, string>)e.OldItems[0];
                Assert.AreEqual(key, oldItem.Key);
                Assert.AreEqual(value, oldItem.Value);
            };

            target.Remove(key);

            Assert.IsTrue(hitChange);
        }

        [TestMethod]
        public void AlternateDictionary()
        {
            var target = new ObservableDictionary<int, string>(new ConcurrentDictionary<int, string>());
            bool hitChange = false;
            const int key = 10;
            const string value = "Hello";

            target.CollectionChanged += (s, e) =>
            {
                hitChange = true;
                Assert.AreSame(target, s);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
                var item = (KeyValuePair<int, string>)e.NewItems[0];
                Assert.AreEqual(key, item.Key);
                Assert.AreEqual(value, item.Value);
            };

            target[key] = value;
            Assert.IsTrue(hitChange);
        }
    }
}

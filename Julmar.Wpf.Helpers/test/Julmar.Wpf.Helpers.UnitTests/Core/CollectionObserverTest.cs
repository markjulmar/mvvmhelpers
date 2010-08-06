using System.Collections.Generic;
using System.Collections.ObjectModel;
using JulMar.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace Julmar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for CollectionObserverTest and is intended
    ///to contain all CollectionObserverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CollectionObserverTests
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

        class ReceiverTest
        {
            public object Sender;
            public List<NotifyCollectionChangedEventArgs> List = new List<NotifyCollectionChangedEventArgs>();

            public void OnObserver(object sender, NotifyCollectionChangedEventArgs e)
            {
                Sender = sender;
                List.Add(e);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BadCollectionTest()
        {
            List<string> stringList = new List<string>();
            CollectionObserver observer = new CollectionObserver(stringList);
            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullCollectionTest()
        {
            CollectionObserver observer = new CollectionObserver(null);
        }

        [TestMethod()]
        public void CollectionObserverTest()
        {
            ObservableCollection<string> coll = new ObservableCollection<string>(new [] { "This","is","a","test"});
            ReceiverTest test = new ReceiverTest();
            CollectionObserver observer = new CollectionObserver(coll);
            observer.CollectionChanged += test.OnObserver;

            coll.RemoveAt(0);
            coll.Add("of");
            coll.Clear();

            Assert.AreEqual(3,test.List.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, test.List[0].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, test.List[1].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, test.List[2].Action);
        }

        [TestMethod()]
        public void DisposeTest()
        {
            ObservableCollection<string> coll = new ObservableCollection<string>(new[] { "This", "is", "a", "test" });
            ReceiverTest test = new ReceiverTest();
            CollectionObserver observer = new CollectionObserver(coll);
            observer.CollectionChanged += test.OnObserver;

            observer.Dispose();
            coll.Clear();
            Assert.AreEqual(0, test.List.Count);
        }

        [TestMethod()]
        public void GCDisposeTest()
        {
            ObservableCollection<string> coll = new ObservableCollection<string>(new[] { "This", "is", "a", "test" });
            WeakReference weakReference = new WeakReference(coll);
            ReceiverTest test = new ReceiverTest();
            CollectionObserver observer = new CollectionObserver(coll);
            observer.CollectionChanged += test.OnObserver;

            observer.Dispose();
            coll = null;
            Assert.AreEqual(0, test.List.Count);
            test = null;
            GC.Collect(2,GCCollectionMode.Forced);

            Assert.IsFalse(weakReference.IsAlive);
        }

        [TestMethod]
        public void BadEventTargetTest()
        {
            ObservableCollection<string> coll = new ObservableCollection<string>(new[] { "This", "is", "a", "test" });
            CollectionObserver observer = new CollectionObserver(coll);
            observer.CollectionChanged += (s, e) => { throw new Exception("boom!"); };
            ReceiverTest test = new ReceiverTest();
            observer.CollectionChanged += test.OnObserver;

            coll.Add("Test");
            coll.Clear();
            Assert.AreEqual(2, test.List.Count);
        }

        [TestMethod]
        public void NoTargetsTest()
        {
            ObservableCollection<string> coll = new ObservableCollection<string>(new[] { "This", "is", "a", "test" });
            CollectionObserver observer = new CollectionObserver(coll);
            coll.Add("Test");
        }
    }
}

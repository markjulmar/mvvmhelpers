using System.Threading.Tasks;
using JulMar.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace JulMar.Wpf.Helpers.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for MTObservableCollectionTest and is intended
    ///to contain all MTObservableCollectionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MTObservableCollectionTest
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
        public void AddRangeTest()
        {
            int didNotify = 0;
            MTObservableCollection<string> coll = new MTObservableCollection<string>();
            coll.CollectionChanged += (s, e) => didNotify++;

            coll.AddRange(new[] { "One", "Two", "Three", "Four" });
            Assert.AreEqual(4, coll.Count);
            Assert.AreEqual(1, didNotify);
        }

        [TestMethod]
        public void MultiAddTest()
        {
            int didNotify = 0;
            MTObservableCollection<int> coll = new MTObservableCollection<int>();
            coll.CollectionChanged += (s, e) => didNotify++;

            coll.CollectionChanged += (s, e) =>
            {
                int currentCount = coll.Count;
                Thread.Sleep(10);
                Assert.AreEqual(currentCount, coll.Count);
            };

            Barrier waitEvent = new Barrier(4);

            Action<object> work = o =>
            {
                waitEvent.SignalAndWait();
                int start = (int)o;
                for (int i = start; i < start + 100; i++)
                {
                    coll.Add(i);
                }
            };

            Task parent = Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < waitEvent.ParticipantCount; i++)
                {
                    Task.Factory.StartNew(work, i*100, TaskCreationOptions.AttachedToParent);
                }
            });

            parent.Wait();

            Assert.AreEqual(400, didNotify);
            Assert.AreEqual(400, coll.Count);
            CollectionAssert.AreEquivalent(Enumerable.Range(0,400).ToArray(), coll);
        }
    }
}

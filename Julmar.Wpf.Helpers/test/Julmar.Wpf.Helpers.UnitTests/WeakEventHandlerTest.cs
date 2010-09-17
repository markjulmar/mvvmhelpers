using JulMar.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for WeakEventHandlerTest and is intended
    ///to contain all WeakEventHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WeakEventHandlerTest
    {
        // Force targets to stay alive by caching in static.
        private static object myTarget;

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

        class TestClass
        {
            public event EventHandler TestEvent;
            public void RaiseTestEvent()
            {
                if (TestEvent != null)
                    TestEvent(this, EventArgs.Empty);
            }

            public Delegate[] GetInvocationList()
            {
                return TestEvent != null ? TestEvent.GetInvocationList() : new Delegate[0];
            }
        }

        class TestTarget
        {
            public Action<int> AddToSum;

            public void Test(object sender, EventArgs e)
            {
                if (AddToSum != null)
                    AddToSum(1);
            }
        }

        [TestMethod()]
        public void WeakEventTest()
        {
            int val = 0;
            
            TestTarget target = new TestTarget {AddToSum = (i) => { val += i; }};
            myTarget = target;
            TestClass tc = new TestClass();
            tc.TestEvent += WeakEventHandler.Create(target.Test, e => tc.TestEvent-=e);

            tc.RaiseTestEvent();
            Assert.AreEqual(1, val);
            Assert.AreEqual(1, tc.GetInvocationList().Length);
            target = null;
            GC.Collect(2, GCCollectionMode.Forced);

            tc.RaiseTestEvent();
            Assert.AreEqual(2, val);
            Assert.AreEqual(1, tc.GetInvocationList().Length);
            myTarget = null;
            GC.Collect(2, GCCollectionMode.Forced);

            val = 0;
            tc.RaiseTestEvent();
            Assert.AreEqual(0, val);
            Assert.AreEqual(0, tc.GetInvocationList().Length);
        }

        class SumEventArgs : EventArgs
        {
            public int Value { get; set; }
        }

        class TestArgClass
        {
            public event EventHandler<SumEventArgs> TestEvent;
            public void RaiseTestEvent(int sum)
            {
                if (TestEvent != null)
                    TestEvent(this, new SumEventArgs {Value = sum});
            }
            
            public Delegate[] GetInvocationList()
            {
                return TestEvent != null ? TestEvent.GetInvocationList() : new Delegate[0];
            }
        }

        class TestArgTarget
        {
            public Action<int> AddToSum;
            public void Test(object sender, SumEventArgs e)
            {
                if (AddToSum != null)
                    AddToSum(e.Value);
            }
        }

        [TestMethod()]
        public void WeakArgsEventTest()
        {
            int val = 0;
            var target = new TestArgTarget { AddToSum = (i) => { val += i; } };
            myTarget = target;

            var tc = new TestArgClass();
            tc.TestEvent += WeakEventHandler.Create<SumEventArgs>(target.Test, e => tc.TestEvent-=e);

            tc.RaiseTestEvent(10);
            Assert.AreEqual(10, val);
            Assert.AreEqual(1, tc.GetInvocationList().Length);

            target = null;
            GC.Collect(2, GCCollectionMode.Forced);
            tc.RaiseTestEvent(10);
            Assert.AreEqual(20, val);
            Assert.AreEqual(1, tc.GetInvocationList().Length);

            myTarget = null;
            GC.Collect(2, GCCollectionMode.Forced);
            
            val = 0;
            GC.Collect(2, GCCollectionMode.Forced);
            tc.RaiseTestEvent(20);
            tc.RaiseTestEvent(20);
            Assert.AreEqual(0, val);
            Assert.AreEqual(0, tc.GetInvocationList().Length);
        }
    }
}

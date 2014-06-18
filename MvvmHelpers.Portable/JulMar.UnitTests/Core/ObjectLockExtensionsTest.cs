using System;
using JulMar.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JulMar.Tests.Core
{
    /// <summary>
    ///This is a test class for ObjectLockExtensionsTest and is intended
    ///to contain all ObjectLockExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObjectLockExtensionsTest
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void UsingLockTestNull()
        {
            object myLock = null;
            myLock.UsingLock(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UsingLockTestNull2()
        {
            object myLock = new object();
            myLock.UsingLock(null);
        }

        [TestMethod()]
        public void UsingTryLockTestFail()
        {
            object myLock = new object();
            int val = 0;
            lock (myLock)
            {
                Action test = () =>
                    {
                        bool rc = myLock.TryUsingLock(1, () => { val++; });
                        Assert.IsFalse(rc);
                        Assert.AreEqual(0, val);
                    };
                var ia = test.BeginInvoke(null, null);
                test.EndInvoke(ia);
                Assert.AreEqual(0, val);
            }
        }

        [TestMethod()]
        public void UsingTryLockTestSuccess()
        {
            object myLock = new object();
            int val = 0;
            IAsyncResult ia;
            Action test = () =>
            {
                bool rc = myLock.TryUsingLock(TimeSpan.FromSeconds(1), () => { val++; });
                Assert.IsTrue(rc);
            };
            lock (myLock)
            {
                ia = test.BeginInvoke(null, null);
            }
            test.EndInvoke(ia);
            Assert.AreEqual(1, val);
        }
    }
}

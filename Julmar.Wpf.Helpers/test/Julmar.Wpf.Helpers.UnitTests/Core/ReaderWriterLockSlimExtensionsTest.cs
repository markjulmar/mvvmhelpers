using JulMar.Core.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for ReaderWriterLockSlimExtensionsTest and is intended
    ///to contain all ReaderWriterLockSlimExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReaderWriterLockSlimExtensionsTest
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
        public void TryUsingReadLockTestNull1()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.UsingReadLock(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryUsingReadLockTestNull2()
        {
            ReaderWriterLockSlim rwl = null;
            rwl.UsingReadLock(null);
        }

        [TestMethod()]
        public void TryUsingReadLockTest()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.UsingReadLock(
                () => {
                        Assert.IsTrue(rwl.IsReadLockHeld);
                        Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                        Assert.IsFalse(rwl.IsWriteLockHeld);
                    });
            Assert.IsFalse(rwl.IsReadLockHeld);
        }

        [TestMethod()]
        public void TryUsingReadLockTestTimeout()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.EnterWriteLock();

            Action test = () =>
                {
                    bool rc = rwl.TryUsingReadLock(1, Assert.Fail);
                    Assert.IsFalse(rwl.IsReadLockHeld);
                    Assert.IsFalse(rc);
                };
            var ia = test.BeginInvoke(null, null);
            test.EndInvoke(ia);
        }

        [TestMethod()]
        public void TryUsingReadLockTestTimespan()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.EnterWriteLock();

            Action test = () =>
                {
                    bool rc = rwl.TryUsingReadLock(TimeSpan.FromSeconds(1), Assert.Fail);
                    Assert.IsFalse(rwl.IsReadLockHeld);
                    Assert.IsFalse(rc);
                };
            var ia = test.BeginInvoke(null, null);
            test.EndInvoke(ia);
        }

        [TestMethod()]
        public void TryUsingReadLockTestTimespanZero()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

            bool rc = rwl.TryUsingReadLock(TimeSpan.Zero,
                () =>
                {
                    Assert.IsTrue(rwl.IsReadLockHeld);
                    Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rwl.IsWriteLockHeld);
                });
            Assert.IsFalse(rwl.IsReadLockHeld);
            Assert.IsTrue(rc);
        }

        [TestMethod()]
        public void TryUsingReadLockTestParameter()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            const string expected = "Hello, World";

            string result = rwl.UsingReadLock(() =>
                {
                    Assert.IsTrue(rwl.IsReadLockHeld);
                    Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rwl.IsWriteLockHeld);
                    return expected;
                });
            Assert.IsFalse(rwl.IsReadLockHeld);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void TryUsingReadLockTestOutParameter()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            const string expected = "Hello, World";
            string result;

            bool rc = rwl.TryUsingReadLock(TimeSpan.Zero,
                () =>
                {
                    Assert.IsTrue(rwl.IsReadLockHeld);
                    Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rwl.IsWriteLockHeld);
                    return expected;
                }, out result);
            Assert.IsFalse(rwl.IsReadLockHeld);
            Assert.IsTrue(rc);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryUsingUpgradeableReadLockTestNull1()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.UsingUpgradeableReadLock(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryUsingUpgradeableReadLockTestNull2()
        {
            ReaderWriterLockSlim rwl = null;
            rwl.UsingUpgradeableReadLock(null);
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTest()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.UsingUpgradeableReadLock(
                () =>
                {
                    Assert.IsFalse(rwl.IsReadLockHeld);
                    Assert.IsTrue(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rwl.IsWriteLockHeld);
                });
            Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTestTimeoutWithRead()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.EnterReadLock();

            Action test = () =>
                              {
                                  bool rc = rwl.TryUsingUpgradeableReadLock(1,
                                    () =>
                                        {
                                            Assert.IsFalse(rwl.IsReadLockHeld);
                                            Assert.IsTrue(rwl.IsUpgradeableReadLockHeld);
                                            Assert.IsFalse(rwl.IsWriteLockHeld);
                                        });
                                  Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                                  Assert.IsTrue(rc);
                              };
            var ia = test.BeginInvoke(null, null);
            test.EndInvoke(ia);
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTestTimeoutFail()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.EnterWriteLock();

            Action test = () =>
                {
                    bool rc = rwl.TryUsingUpgradeableReadLock(1, Assert.Fail);
                    Assert.IsFalse(rwl.IsReadLockHeld);
                    Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rc);
                };
            var ia = test.BeginInvoke(null, null);
            test.EndInvoke(ia);
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTestTimespan()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.EnterWriteLock();

            Action test = () =>
                {
                    bool rc = rwl.TryUsingUpgradeableReadLock(TimeSpan.FromSeconds(1), Assert.Fail);
                    Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rc);
                };
            var ia = test.BeginInvoke(null, null);
            ia.AsyncWaitHandle.WaitOne();
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTestTimespanSuccess()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.EnterWriteLock();
            int val = 0;

            Action test = () =>
            {
                bool rc = rwl.TryUsingUpgradeableReadLock(TimeSpan.FromSeconds(10), () => { val++; });
                Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                Assert.IsTrue(rc);
            };
            
            var ia = test.BeginInvoke(null, null);
            Thread.Sleep(1);
            rwl.ExitWriteLock();
            
            ia.AsyncWaitHandle.WaitOne();
            Assert.AreEqual(1,val);
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTestTimespanZero()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

            bool rc = rwl.TryUsingUpgradeableReadLock(TimeSpan.Zero,
                () =>
                {
                    Assert.IsFalse(rwl.IsReadLockHeld);
                    Assert.IsTrue(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rwl.IsWriteLockHeld);
                });
            Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
            Assert.IsTrue(rc);
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTestParameter()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            const string expected = "Hello, World";

            string result = rwl.UsingUpgradeableReadLock(() =>
            {
                Assert.IsTrue(rwl.IsUpgradeableReadLockHeld);
                return expected;
            });
            Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTestOutParameter()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            const string expected = "Hello, World";
            string result;

            bool rc = rwl.TryUsingUpgradeableReadLock(TimeSpan.Zero, () => expected, out result);
            Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
            Assert.IsTrue(rc);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void TryUsingUpgradableReadLockTestToWrite()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.UsingUpgradeableReadLock(
                () =>
                {
                    Assert.IsTrue(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rwl.IsWriteLockHeld);
                    rwl.UsingWriteLock(() =>
                                           {
                                               Assert.IsTrue(rwl.IsWriteLockHeld);
                                               Assert.IsTrue(rwl.IsUpgradeableReadLockHeld);
                                           });
                    Assert.IsTrue(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rwl.IsWriteLockHeld);
                });
            Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryUsingWriteLockTestNull1()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.UsingWriteLock(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryUsingWriteLockTestNull2()
        {
            ReaderWriterLockSlim rwl = null;
            rwl.UsingWriteLock(null);
        }

        [TestMethod()]
        public void TryUsingWriteLockTest()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.UsingWriteLock(
                () =>
                {
                    Assert.IsFalse(rwl.IsReadLockHeld);
                    Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsTrue(rwl.IsWriteLockHeld);
                });
            Assert.IsFalse(rwl.IsWriteLockHeld);
        }

        [TestMethod()]
        public void TryUsingWriteLockTestTimeout()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.EnterWriteLock();

            Action test = () =>
            {
                bool rc = rwl.TryUsingWriteLock(1, Assert.Fail);
                Assert.IsFalse(rwl.IsWriteLockHeld);
                Assert.IsFalse(rc);
            };
            var ia = test.BeginInvoke(null, null);
            ia.AsyncWaitHandle.WaitOne();
        }

        [TestMethod()]
        public void TryUsingWriteLockTestTimespan()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            rwl.EnterWriteLock();

            Action test = () =>
            {
                bool rc = rwl.TryUsingWriteLock(TimeSpan.FromSeconds(1), Assert.Fail);
                Assert.IsFalse(rwl.IsWriteLockHeld);
                Assert.IsFalse(rc);
            };
            var ia = test.BeginInvoke(null, null);
            ia.AsyncWaitHandle.WaitOne();
        }

        [TestMethod()]
        public void TryUsingWriteLockTestTimespanZero()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

            bool rc = rwl.TryUsingWriteLock(TimeSpan.Zero,
                () =>
                {
                    Assert.IsTrue(rwl.IsWriteLockHeld);
                    Assert.IsFalse(rwl.IsUpgradeableReadLockHeld);
                    Assert.IsFalse(rwl.IsReadLockHeld);
                });
            Assert.IsFalse(rwl.IsWriteLockHeld);
            Assert.IsTrue(rc);
        }

        [TestMethod()]
        public void TryUsingWriteLockTestParameter()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            const string expected = "Hello, World";

            string result = rwl.UsingWriteLock(() => expected);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void TryUsingWriteLockTestOutParameter()
        {
            ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
            const string expected = "Hello, World";
            string result;

            bool rc = rwl.TryUsingReadLock(TimeSpan.Zero, () => expected, out result);
            Assert.IsTrue(rc);
            Assert.AreEqual(expected, result);
        }
    }
}

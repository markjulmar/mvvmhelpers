using JulMar.Core.Concurrency;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;

namespace Julmar.Metro.Tests
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

        [TestMethod()]
        public void UsingLockTestNull()
        {
            object myLock = null;
            Assert.ThrowsException<ArgumentNullException>(() => myLock.UsingLock(null));
        }

        [TestMethod()]
        public void UsingLockTestNull2()
        {
            object myLock = new object();
            Assert.ThrowsException<ArgumentNullException>(() => myLock.UsingLock(null));
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

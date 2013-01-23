using System;
using JulMar.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JulMar.Wpf.Helpers.UnitTests.Core
{
    [TestClass]
    public class WeakDelegateTests
    {
        [TestMethod]
        public void InstanceReferenceStillAlive()
        {
            TestClass tc = new TestClass();
            WeakDelegate wd = new WeakDelegate(new Func<int>(tc.SomeMethod));

            GC.Collect();
            Assert.IsTrue(wd.IsAlive);
            Assert.IsTrue(wd.Equals(new Func<int>(tc.SomeMethod)));
        }

        [TestMethod]
        public void InstanceReferenceIsDead()
        {
            var wd = CreateDelegate();

            GC.Collect();

            Assert.IsFalse(wd.IsAlive);
        }

        [TestMethod]
        public void StaticReferenceIsAlive()
        {
            var wd = new WeakDelegate(new Func<int>(TestClass.SomeStaticMethod));
            GC.Collect();
            Assert.IsTrue(wd.IsAlive);
            Assert.IsTrue(wd.Equals(new Func<int>(TestClass.SomeStaticMethod)));
        }

        public WeakDelegate CreateDelegate()
        {
            TestClass tc = new TestClass();
            return new WeakDelegate(new Func<int>(tc.SomeMethod));
        }

        public class TestClass
        {
            public int SomeMethod()
            {
                return 42;
            }

            public static int SomeStaticMethod()
            {
                return 43;
            }
        }

    }
}

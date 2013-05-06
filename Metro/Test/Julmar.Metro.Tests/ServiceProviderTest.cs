using JulMar.Core.Interfaces;
using JulMar.Core.Services;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Composition;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for ServiceProviderTest and is intended
    ///to contain all ServiceProviderTest Unit Tests
    ///</summary>
    [TestClass]
    public class ServiceProviderTest
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            const string targetString = "This is a test";

            IServiceLocator sp = ServiceLocator.Instance;

            sp.Add(typeof(string), targetString);

            var result = sp.Resolve<string>();
            Assert.AreEqual(targetString, result);
        }

        /// <summary>
        ///A test for Remove
        ///</summary>
        [TestMethod()]
        public void RemoveTest()
        {
            const string targetString = "This is a test";

            IServiceLocator sp = ServiceLocator.Instance;

            sp.Add(typeof(string), targetString);

            var result = sp.Resolve<string>();
            Assert.AreEqual(targetString, result);

            sp.Remove(typeof(string));
            result = sp.Resolve<string>();
            Assert.AreEqual(null, result);
        }

        /// <summary>
        ///A test for Add with subtype
        ///</summary>
        [TestMethod()]
        public void InvalidCastTest()
        {
            object obj = new object();
            IServiceLocator sp = ServiceLocator.Instance;

            sp.Add(typeof(IComparable), obj);

            var result = sp.Resolve<string>();
            Assert.AreEqual(null, result);

            Assert.ThrowsException<InvalidCastException>(() => sp.Resolve<IComparable>());
        }

        [Export(typeof(MyServiceClass))]
        class MyServiceClass
        {
        }

        [TestMethod]
        public void TestMefLoaderWithServiceLocator()
        {
            IServiceLocator sp = ServiceLocator.Instance;
            Assert.IsNotNull(sp);

            IServiceLocator sp2 = DynamicComposer.Instance.GetExportedValue<IServiceLocator>();
            Assert.AreSame(sp,sp2);
        }

        [TestMethod]
        public void TestMefLoaderWithServiceProvider()
        {
            IServiceLocator sp = ServiceLocator.Instance;
            Assert.IsNotNull(sp);

            IServiceProvider sp2 = DynamicComposer.Instance.GetExportedValue<IServiceProvider>();
            Assert.AreSame(sp, sp2);
        }

        [TestMethod]
        public void TestMyServiceExport()
        {
            IServiceLocator sp = ServiceLocator.Instance;
            Assert.IsNotNull(sp);

            var msc = sp.Resolve<MyServiceClass>();
            Assert.IsNotNull(msc);
        }

        interface IMyServiceClass2 {}

        [Export(typeof(IMyServiceClass2))]
        class MyServiceClass2 : IMyServiceClass2
        {
        }

        [TestMethod]
        public void TestMyServiceExportInterface()
        {
            IServiceLocator sp = ServiceLocator.Instance;
            Assert.IsNotNull(sp);

            var msc = sp.Resolve<IMyServiceClass2>();
            Assert.IsNotNull(msc);
        }

        public interface IMyServiceClassBase {}

        public interface IMyServiceClass3 : IMyServiceClassBase { }

        [Export(typeof(IMyServiceClass3))]
        class MyServiceClass3 : IMyServiceClass3
        {
        }

        [TestMethod]
        public void TestMyServiceExportInterfaceInherited()
        {
            IServiceLocator sp = ServiceLocator.Instance;
            Assert.IsNotNull(sp);

            var msc = sp.Resolve<IMyServiceClassBase>();
            Assert.IsNull(msc);
        }

        [Import] public IMyServiceClass3 TestImport {get;set;}

        [TestMethod]
        public void ImportTestClass()
        {
            DynamicComposer.Instance.Compose(this);
            Assert.IsNotNull(TestImport);
        }
    }
}

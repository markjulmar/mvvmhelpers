using System.ComponentModel.Composition;
using JulMar.Core;
using JulMar.Core.Interfaces;
using JulMar.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for ServiceProviderTest and is intended
    ///to contain all ServiceProviderTest Unit Tests
    ///</summary>
    [TestClass()]
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

            ServiceLocator sp = new ServiceLocator();

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

            ServiceLocator sp = new ServiceLocator();

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
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidCastTest()
        {
            object obj = new object();
            ServiceLocator sp = new ServiceLocator();

            sp.Add(typeof(IComparable), obj);

            var result = sp.Resolve<string>();
            Assert.AreEqual(null, result);

            sp.Resolve<IComparable>();
        }

        [ExportService(typeof(MyServiceClass))]
        class MyServiceClass
        {
        }

        [TestMethod]
        public void TestMefLoader()
        {
            IServiceProviderEx sp = DynamicComposer.Instance.GetExportedValue<IServiceProviderEx>();
            Assert.IsNotNull(sp);

            IServiceProvider sp2 = DynamicComposer.Instance.GetExportedValue<IServiceProvider>();
            Assert.AreSame(sp,sp2);
        }

        [TestMethod]
        public void TestMyServiceExport()
        {
            IServiceProviderEx sp = DynamicComposer.Instance.GetExportedValue<IServiceProviderEx>();
            Assert.IsNotNull(sp);

            var msc = sp.Resolve<MyServiceClass>();
            Assert.IsNotNull(msc);
        }

        interface IMyServiceClass2 {}

        [ExportService(typeof(IMyServiceClass2))]
        class MyServiceClass2 : IMyServiceClass2
        {
        }

        [TestMethod]
        public void TestMyServiceExportInterface()
        {
            IServiceProviderEx sp = DynamicComposer.Instance.GetExportedValue<IServiceProviderEx>();
            Assert.IsNotNull(sp);

            var msc = sp.Resolve<IMyServiceClass2>();
            Assert.IsNotNull(msc);
        }

        interface IMyServiceClassBase {}

        [InheritedExport]
        interface IMyServiceClass3 : IMyServiceClassBase { }

        [ExportService(typeof(IMyServiceClass3))]
        class MyServiceClass3 : IMyServiceClass3
        {
        }

        [TestMethod]
        public void TestMyServiceExportInterfaceInherited()
        {
            IServiceProviderEx sp = DynamicComposer.Instance.GetExportedValue<IServiceProviderEx>();
            Assert.IsNotNull(sp);

            var msc = sp.Resolve<IMyServiceClassBase>();
            Assert.IsNull(msc);
        }

        [Import] private IMyServiceClass3 TestImport = null;

        [TestMethod]
        public void ImportTestClass()
        {
            DynamicComposer.Instance.Compose(this);
            Assert.IsNotNull(TestImport);
        }
    }
}

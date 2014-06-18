using System;
using JulMar.Core;
using JulMar.Interfaces;
using JulMar.Services;
using JulMar.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: ExportService(typeof(ServiceProviderTest.MyServiceClass))]
[assembly: ExportService(typeof(ServiceProviderTest.IMyServiceClass2), typeof(JulMar.Tests.Core.ServiceProviderTest.MyServiceClass2))]

namespace JulMar.Tests.Core
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

            IServiceLocator sp = new ServiceLocator();

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

            IServiceLocator sp = new ServiceLocator();

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
        [ExpectedException(typeof(InvalidCastException))]
        public void InvalidCastTest()
        {
            object obj = new object();
            IServiceLocator sp = new ServiceLocator();

            sp.Add(typeof(IComparable), obj);

            var result = sp.Resolve<string>();
            Assert.AreEqual(null, result);

            sp.Resolve<IComparable>();
        }

        public class MyServiceClass
        {
        }

        [TestMethod]
        public void TestMyServiceExport()
        {
            IServiceLocator sp = new ServiceLocator();

            var msc = sp.Resolve<MyServiceClass>();
            Assert.IsNotNull(msc);
        }

        public interface IMyServiceClass2 {}
        public class MyServiceClass2 : IMyServiceClass2
        {
        }

        [TestMethod]
        public void TestMyServiceExportInterface()
        {
            IServiceLocator sp = new ServiceLocator();
            Assert.IsNotNull(sp);

            var msc = sp.Resolve<IMyServiceClass2>();
            Assert.IsNotNull(msc);
        }
    }
}

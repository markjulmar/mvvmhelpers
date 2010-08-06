using JulMar.Windows.Mvvm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Julmar.Wpf.Helpers.UnitTests
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

            ServiceProvider sp = new ServiceProvider();

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

            ServiceProvider sp = new ServiceProvider();

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
            ServiceProvider sp = new ServiceProvider();

            sp.Add(typeof(IComparable), obj);

            var result = sp.Resolve<string>();
            Assert.AreEqual(null, result);

            sp.Resolve<IComparable>();
        }
    }
}

using JulMar.Core.Extensions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Linq;

namespace Julmar.Metro.Tests
{
    /// <summary>
    ///This is a test class for CollectionObserverTest and is intended
    ///to contain all CollectionObserverTest Unit Tests
    ///</summary>
    [TestClass]
    public class CollectionExtensionSwapTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void SwapTestBadStartingIndexIsNegative()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Enumerable.Range(0, 5).ToArray().Swap(-1, 0));
        }

        [TestMethod]
        public void SwapTestBadStartingIndexIsTooBig()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Enumerable.Range(0, 5).ToArray().Swap(5, 0));
        }

        [TestMethod]
        public void SwapTestBadDestIndexIsNegative()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Enumerable.Range(0, 5).ToArray().Swap(0, -1));
        }

        [TestMethod]
        public void SwapTestBadDestIndexIsTooBig()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => Enumerable.Range(0, 5).ToArray().Swap(0, 9));
        }

        [TestMethod]
        public void SwapTestBadIsNull()
        {
            byte[] data = null;
            Assert.ThrowsException<NullReferenceException>(() => data.Swap(0, 1));
        }

        [TestMethod]
        public void SwapTestBackward()
        {
            int[] data = {1, 2, 3};
            data.Swap(2, 0);
            CollectionAssert.AreEqual(new[] { 3, 2, 1 }, data);
        }

        [TestMethod]
        public void SwapTestForward()
        {
            int[] data = { 1, 2, 3, 4, 5 };
            data.Swap(2, 4);
            CollectionAssert.AreEqual(new[] { 1, 2, 5, 4, 3 }, data);
        }

        [TestMethod]
        public void SwapTestSameIndex()
        {
            int[] data = { 1, 2, 3, 4, 5 };
            data.Swap(2, 2);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, data);
        }
    }
}

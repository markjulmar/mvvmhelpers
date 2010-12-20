using JulMar.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using JulMar.Core.Collections;

namespace JulMar.Wpf.Helpers.UnitTests.Core
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SwapTestBadStartingIndexIsNegative()
        {
            Enumerable.Range(0, 5).ToArray().Swap(-1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SwapTestBadStartingIndexIsTooBig()
        {
            Enumerable.Range(0, 5).ToArray().Swap(5, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SwapTestBadDestIndexIsNegative()
        {
            Enumerable.Range(0, 5).ToArray().Swap(0, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SwapTestBadDestIndexIsTooBig()
        {
            Enumerable.Range(0, 5).ToArray().Swap(0, 9);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void SwapTestBadIsNull()
        {
            byte[] data = null;
            data.Swap(0, 1);
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

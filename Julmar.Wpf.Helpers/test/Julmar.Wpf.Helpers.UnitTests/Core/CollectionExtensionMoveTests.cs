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
    public class CollectionExtensionMoveTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveTestBadStartingIndexIsNegative()
        {
            Enumerable.Range(0, 5).ToArray().MoveRange(-1, 4, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveTestBadStartingIndexIsTooBig()
        {
            Enumerable.Range(0, 5).ToArray().MoveRange(5, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveTestBadDestIndexIsNegative()
        {
            Enumerable.Range(0, 5).ToArray().MoveRange(0, 4, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveTestBadDestIndexIsTooBig()
        {
            Enumerable.Range(0, 5).ToArray().MoveRange(0, 1, 9);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MoveTestBadCount()
        {
            Enumerable.Range(0, 5).ToArray().MoveRange(0, 6, 1);
        }


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void MoveTestBadIsNull()
        {
            byte[] data = null;
            data.MoveRange(0, 1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void MoveTestWithArray()
        {
            Enumerable.Range(0, 5).ToArray().MoveRange(1, 4, 0);
        }

        [TestMethod]
        public void MoveTestBackward()
        {
            var data = Enumerable.Range(0, 5).ToList();
            data.MoveRange(1, 4, 0);
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 0 }, data);
        }

        [TestMethod]
        public void MoveTestForward()
        {
            var data = Enumerable.Range(0, 5).ToList();
            data.MoveRange(0, 4, 1);
            CollectionAssert.AreEquivalent(new[] { 4, 0, 1, 2, 3 }, data);
        }

        [TestMethod]
        public void MoveTestSingleForward()
        {
            var data = Enumerable.Range(0, 5).ToList();
            data.MoveRange(0, 1, 1);
            CollectionAssert.AreEquivalent(new[] { 1, 0, 2, 3, 4 }, data);
        }

        [TestMethod]
        public void MoveTestSingleBackward()
        {
            var data = Enumerable.Range(0, 5).ToList();
            data.MoveRange(4, 1, 0);
            CollectionAssert.AreEquivalent(new[] { 4, 0, 1, 2, 3 }, data);
        }

        [TestMethod]
        public void MoveTestSameIndex()
        {
            int[] data = { 1, 2, 3, 4, 5 };
            data.MoveRange(2, 2, 2);
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, data);
        }

        [TestMethod]
        public void MoveTestZeroCount()
        {
            int[] data = { 1, 2, 3, 4, 5 };
            data.MoveRange(0, 0, 1);
            CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using JulMar.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JulMar.Tests.Core
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
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 0 }, data);
        }

        [TestMethod]
        public void MoveTestForward()
        {
            var data = Enumerable.Range(0, 5).ToList();
            data.MoveRange(0, 4, 1);
            CollectionAssert.AreEqual(new[] { 4, 0, 1, 2, 3 }, data);
        }

        [TestMethod]
        public void MoveTestSingleForward()
        {
            var data = Enumerable.Range(0, 5).ToList();
            data.MoveRange(0, 1, 1);
            CollectionAssert.AreEqual(new[] { 1, 0, 2, 3, 4 }, data);
        }

        [TestMethod]
        public void MoveMultipleTestSingleForward()
        {
            var data = Enumerable.Range(0, 10).ToList();
            data.MoveRange(2, 2, 3);
            CollectionAssert.AreEqual(new[] { 0, 1, 4, 2, 3, 5, 6, 7, 8, 9 }, data);
        }

        [TestMethod]
        public void MoveTestOverlapForward()
        {
            var data = Enumerable.Range(0, 10).ToList();
            data.MoveRange(2, 4, 3);
            CollectionAssert.AreEqual(new[] { 0, 1, 6, 2, 3, 4, 5, 7, 8, 9 }, data);
        }

        [TestMethod]
        public void MoveTestOverlapBackward()
        {
            var data = Enumerable.Range(0, 10).ToList();
            data.MoveRange(4, 4, 2);
            CollectionAssert.AreEqual(new[] { 0, 1, 4, 5, 6, 7, 2, 3, 8, 9 }, data);
        }

        [TestMethod]
        public void MoveTestSingleBackward()
        {
            var data = Enumerable.Range(0, 5).ToList();
            data.MoveRange(4, 1, 0);
            CollectionAssert.AreEqual(new[] { 4, 0, 1, 2, 3 }, data);
        }

        [TestMethod]
        public void MoveTestMultiMiddle()
        {
            var data = new List<int>(new[] {0, 0, 0, 1, 2, 3, 0, 0, 0, 4, 5, 6});
            data.MoveRange(3, 3, 6);
            CollectionAssert.AreEqual(new[] { 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6 }, data);
        }

        [TestMethod]
        public void MoveTestSameIndex()
        {
            int[] data = { 1, 2, 3, 4, 5 };
            data.MoveRange(2, 2, 2);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, data);
        }

        [TestMethod]
        public void MoveTestZeroCount()
        {
            int[] data = { 1, 2, 3, 4, 5 };
            data.MoveRange(0, 0, 1);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, data);
        }
    }
}

using JulMar.Windows.Undo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JulMar.Windows.Interfaces;
using System.Collections.Generic;

namespace Julmar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for UndoManagerTest and is intended
    ///to contain all UndoManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UndoManagerTest
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        class MockUndo : ISupportUndo
        {
            public bool undoCalled, redoCalled;

            public MockUndo(bool canRedo=false) { CanRedo = canRedo; }
            public void Undo() { undoCalled = true; }
            public bool CanRedo { get; private set; }
            public void Redo() { redoCalled = true; }

            public int Value { get; set; }
        }

        [TestMethod]
        public void CanUndoTest()
        {
            UndoManager undoManager = new UndoManager();
            Assert.IsFalse(undoManager.CanUndo);

            undoManager.Add(new MockUndo());
            Assert.IsTrue(undoManager.CanUndo);
            undoManager.Undo();
            Assert.IsFalse(undoManager.CanUndo);
        }

        [TestMethod]
        public void UndoTest()
        {
            UndoManager undoManager = new UndoManager();
            MockUndo undo1 = new MockUndo();
            MockUndo undo2 = new MockUndo();

            undoManager.Add(undo1);
            undoManager.Add(undo2);

            undoManager.Undo();
            Assert.IsTrue(undo2.undoCalled);
            Assert.IsFalse(undo1.undoCalled);
            Assert.IsTrue(undoManager.CanUndo);
        }

        /// <summary>
        ///A test for CanRedo
        ///</summary>
        [TestMethod()]
        public void CanRedoTest()
        {
            UndoManager undoManager = new UndoManager();
            Assert.IsFalse(undoManager.CanUndo);

            undoManager.Add(new MockUndo(false));
            Assert.IsTrue(undoManager.CanUndo);
            undoManager.Undo();
            Assert.IsFalse(undoManager.CanRedo);
            undoManager.Add(new MockUndo(true));
            Assert.IsTrue(undoManager.CanUndo);
            undoManager.Undo();
            Assert.IsFalse(undoManager.CanUndo);
            Assert.IsTrue(undoManager.CanRedo);
            undoManager.Redo();
            Assert.IsTrue(undoManager.CanUndo);
            Assert.IsFalse(undoManager.CanRedo);
        }

        [TestMethod]
        public void RedoTest()
        {
            UndoManager undoManager = new UndoManager();
            MockUndo undo1 = new MockUndo(true);

            undoManager.Add(undo1);
            undoManager.Undo();
            Assert.IsTrue(undo1.undoCalled);
            Assert.IsFalse(undo1.redoCalled);
            Assert.IsTrue(undoManager.CanRedo);
            undoManager.Redo();
            Assert.IsTrue(undo1.redoCalled);
            Assert.IsFalse(undoManager.CanRedo);
        }

        /// <summary>
        ///A test for MaxSupportedOperations
        ///</summary>
        [TestMethod()]
        public void MaxSupportedOperationsTest()
        {
            UndoManager target = new UndoManager {MaxSupportedOperations = 5};
            List<MockUndo> listUndoOps = new List<MockUndo>();

            for (int i = 0; i < 10; i++)
            {
                MockUndo undoObj = new MockUndo() {Value = i};
                target.Add(undoObj);
                listUndoOps.Add(undoObj);
            }

            int count = 0;
            while (target.CanUndo)
            {
                target.Undo();
                count++;
            }

            // Should have called final 5.. skipped first 5.
            for (int i = 0; i < 5; i++)
                Assert.IsFalse(listUndoOps[i].undoCalled);
            for (int i = 5; i < 10; i++)
                Assert.IsTrue(listUndoOps[i].undoCalled);

            Assert.AreEqual(count, target.MaxSupportedOperations);
        }
    }
}

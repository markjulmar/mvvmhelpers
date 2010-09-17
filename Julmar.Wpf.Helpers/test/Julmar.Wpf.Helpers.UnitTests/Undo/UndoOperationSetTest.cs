using JulMar.Core.Interfaces;
using JulMar.Core.Undo;
using JulMar.Windows.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for UndoOperationSetTest and is intended
    ///to contain all UndoOperationSetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UndoOperationSetTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
            public MockUndo(bool canRedo = false) { CanRedo = canRedo; }
            public void Undo() { undoCalled = true; }
            public bool CanRedo { get; private set; }
            public void Redo() { redoCalled = true; }
        }

        /// <summary>
        ///A test for Undo
        ///</summary>
        [TestMethod]
        public void UndoTest()
        {
            MockUndo m1 = new MockUndo(true);
            MockUndo m2 = new MockUndo(true);

            UndoOperationSet target = new UndoOperationSet();
            target.Add(m1);
            target.Add(m2);

            target.Undo();

            Assert.IsTrue(m1.undoCalled);
            Assert.IsTrue(m2.undoCalled);
        }

        [TestMethod]
        public void UndoOrderTest()
        {
            int finalValue = 0;
            MockUndo m1 = new MockUndo(true);

            UndoOperationSet target = new UndoOperationSet();
            target.Add(m1);
            target.Add(new DelegateUndo(() => finalValue = 1));
            target.Add(new DelegateUndo(() => finalValue = 2));

            target.Undo();

            Assert.IsTrue(m1.undoCalled);
            Assert.AreEqual(1, finalValue);
        }

        [TestMethod()]
        public void CanRedoTest_OK()
        {
            MockUndo m1 = new MockUndo(true);
            MockUndo m2 = new MockUndo(true);

            UndoOperationSet target = new UndoOperationSet();
            target.Add(m1);
            target.Add(m2);

            Assert.IsTrue(target.CanRedo);
        }

        [TestMethod()]
        public void CanRedoTest_NAK()
        {
            MockUndo m1 = new MockUndo(true);
            MockUndo m2 = new MockUndo(false);

            UndoOperationSet target = new UndoOperationSet();
            target.Add(m1);
            target.Add(m2);

            Assert.IsFalse(target.CanRedo);
        }

        /// <summary>
        ///A test for Redo
        ///</summary>
        [TestMethod()]
        public void RedoTest()
        {
            MockUndo m1 = new MockUndo(true);
            MockUndo m2 = new MockUndo(true);

            UndoOperationSet target = new UndoOperationSet();
            target.Add(m1);
            target.Add(m2);

            target.Redo();
            Assert.IsTrue(m1.redoCalled);
            Assert.IsTrue(m2.redoCalled);
        }


    }
}

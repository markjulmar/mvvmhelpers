using System;
using JulMar.Windows.Mvvm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JulMar.Wpf.Helpers.UnitTests.MVVM
{
    /// <summary>
    ///This is a test class for ShallowCopyEditableObjectTest and is intended
    ///to contain all ShallowCopyEditableObjectTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ShallowCopyEditableObjectTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        class TestClass1 : EditingViewModel
        {
            private string _s1;
            public string S1
            {
                get { return _s1; }
                set { _s1 = value; }
            }

            public int i1 = 30;
            public double db1 = 3.14;
            public DateTime dt = new DateTime(2010,12,25,12,59,59);

            public TestClass1()
            {
                S1 = "Test123";
            }
        }

        /// <summary>
        ///A test for OnBeginEdit
        ///</summary>
        [TestMethod()]
        public void OnNoCommitEditTest()
        {
            ShallowCopyEditableObject target = new ShallowCopyEditableObject();
            TestClass1 tc = new TestClass1();

            target.OnBeginEdit(tc);
            tc.S1 = "Hello";
            tc.i1 = 10;
            tc.db1 = 1.00;
            tc.dt = DateTime.Now;
            target.OnEndEdit(tc, false);

            Assert.AreEqual("Test123", tc.S1);
            Assert.AreEqual(30, tc.i1);
            Assert.AreEqual(3.14, tc.db1);
            Assert.AreEqual(new DateTime(2010, 12, 25, 12, 59, 59), tc.dt);
        }

        /// <summary>
        ///A test for OnBeginEdit
        ///</summary>
        [TestMethod()]
        public void OnCommitEditTest()
        {
            ShallowCopyEditableObject target = new ShallowCopyEditableObject();
            TestClass1 tc = new TestClass1();

            DateTime td = DateTime.Now;

            target.OnBeginEdit(tc);
            tc.S1 = "Hello";
            tc.i1 = 10;
            tc.db1 = 1.00;
            tc.dt = td;
            target.OnEndEdit(tc, true);

            Assert.AreEqual("Hello", tc.S1);
            Assert.AreEqual(10, tc.i1);
            Assert.AreEqual(1.0, tc.db1);
            Assert.AreEqual(td, tc.dt);
        }

        /// <summary>
        ///A test for OnBeginEdit
        ///</summary>
        [TestMethod()]
        public void OnPartialCommitEditTest()
        {
            ShallowCopyEditableObject target = new ShallowCopyEditableObject();
            TestClass1 tc = new TestClass1();

            tc.FieldPredicate = fi => fi.Name != "_s1";

            DateTime td = DateTime.Now;

            target.OnBeginEdit(tc);
            tc.S1 = "Hello";
            tc.i1 = 10;
            tc.db1 = 1.00;
            tc.dt = td;
            target.OnEndEdit(tc, false);

            Assert.AreEqual("Hello", tc.S1);
            Assert.AreEqual(30, tc.i1);
            Assert.AreEqual(3.14, tc.db1);
            Assert.AreEqual(new DateTime(2010, 12, 25, 12, 59, 59), tc.dt);
        }
    }
}

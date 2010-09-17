using System.ComponentModel;
using JulMar.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for ObservableObjectTest and is intended
    ///to contain all ObservableObjectTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObservableObjectTest
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

        class ObjectTest : ObservableObject
        {
            private int _value;
            public int Value
            {
                get { return _value; }
                set { ChangeValue("Value", ref _value, value); }
            }

            private string _sValue;
            public string SValue
            {
                get { return _sValue; }
                set { _sValue = value; OnPropertyChanged("SValue", value); }
            }
        }

        [TestMethod()]
        public void ChangeValueTest()
        {
            bool called = false;
            PropertyChangedEventArgsEx epc = null;
            ObjectTest tst = new ObjectTest();
            tst.PropertyChanged+= (s, e) => { called = true; epc = (PropertyChangedEventArgsEx)e; };
            tst.Value = 10;

            Assert.IsTrue(called);
            Assert.AreEqual(0, epc.OldValue);
            Assert.AreEqual(10, epc.NewValue);
            Assert.IsTrue(epc.HasNewValue);
            Assert.IsTrue(epc.HasOldValue);
        }

        [TestMethod()]
        public void OnPropertyChangedTest_NewValue()
        {
            bool called = false;
            PropertyChangedEventArgsEx epc = null;
            ObjectTest tst = new ObjectTest();
            tst.PropertyChanged += (s, e) => { called = true; epc = (PropertyChangedEventArgsEx)e; };
            tst.SValue = "10";

            Assert.IsTrue(called);
            Assert.AreEqual("SValue", epc.PropertyName);
            Assert.IsTrue(epc.HasNewValue);
            Assert.IsFalse(epc.HasOldValue);
            Assert.AreEqual("10", epc.NewValue);
        }

        [TestMethod()]
        public void OnPropertyChangedTest()
        {
            bool called = false;
            PropertyChangedEventArgs epc = null;
            ObjectTest tst = new ObjectTest();
            tst.PropertyChanged += (s, e) => { called = true; epc = (PropertyChangedEventArgs)e; };
            tst.Value = 10;

            Assert.IsTrue(called);
            Assert.AreEqual("Value", epc.PropertyName);
        }

        [TestMethod()]
        public void OnPropertyChanged_MT()
        {
            int calledCount = 0;
            PropertyChangedEventArgsEx epc = null;
            ObjectTest tst = new ObjectTest();
            tst.PropertyChanged += (s, e) => { if (Interlocked.Increment(ref calledCount) == 2) epc = (PropertyChangedEventArgsEx)e; };

            ManualResetEvent evt = new ManualResetEvent(false);
            Action<int> testAction = i => { evt.WaitOne(); tst.Value = i; };
            for (int i = 1; i < 10; i++)
                testAction.BeginInvoke(i, testAction.EndInvoke, null);

            evt.Set();
            while (calledCount != 9)
                Thread.Sleep(1);

            Assert.AreEqual("Value", epc.PropertyName);
            Assert.AreNotSame(tst.Value, epc.NewValue);
        }

    }
}

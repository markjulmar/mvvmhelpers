using System.ComponentModel;
using JulMar.Core;
using JulMar.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        class ObjectTest : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public ObjectTest(bool doRegister = true)
            {
                if (doRegister)
                    this.RegisterPropertyChangedHandler(() => PropertyChanged);
            }

            public int Value
            {
                get { return this.GetBackingValue(() => Value); }
                set { this.SetBackingValue(() => Value, value, true); }
            }

            public string SValue
            {
                get { return this.GetBackingValue(() => SValue); }
                set { this.SetBackingValue(() => SValue, value);}
            }
        }

        [TestMethod()]
        public void ChangeValueTest()
        {
            bool called = false;
            PropertyChangedEventArgs epc = null;
            ObjectTest tst = new ObjectTest();
            tst.PropertyChanged+= (s, e) => { called = true; epc = e; };
            tst.Value = 10;

            Assert.IsTrue(called);
            Assert.AreEqual("Value", epc.PropertyName);
            Assert.AreEqual(10, tst.Value);
        }

        [TestMethod()]
        public void NoChangeValueTest()
        {
            bool called = false;
            PropertyChangedEventArgs epc = null;
            ObjectTest tst = new ObjectTest() { Value = 10 };
            tst.PropertyChanged += (s, e) => { called = true; epc = e; };
            tst.Value = 10;

            Assert.IsFalse(called);
            Assert.AreEqual(10, tst.Value);
        }

        [TestMethod()]
        public void NoRegisterValueTest()
        {
            bool called = false;
            PropertyChangedEventArgs epc = null;
            ObjectTest tst = new ObjectTest(false) { Value = 1 };
            tst.PropertyChanged += (s, e) => { called = true; epc = e; };
            tst.Value = 10;

            Assert.IsTrue(called);
            Assert.AreEqual("Value", epc.PropertyName);
            Assert.AreEqual(10, tst.Value);
        }

        [TestMethod()]
        public void ExtendedChangeValueTest()
        {
            bool called = false;
            PropertyChangedEventArgs epc = null;
            ObjectTest tst = new ObjectTest() { Value = 10 };
            tst.PropertyChanged += (s, e) => { called = true; epc = e; };
            tst.Value = 20;

            Assert.IsTrue(called);
            Assert.AreEqual(20, tst.Value);
            Assert.IsInstanceOfType(epc, typeof(PropertyChangedEventArgsEx));

            var epcx = (PropertyChangedEventArgsEx) epc;
            Assert.AreEqual(10, (int) epcx.OldValue);
            Assert.AreEqual(20, (int) epcx.NewValue);
            Assert.IsTrue(epcx.HasNewValue);
            Assert.IsTrue(epcx.HasOldValue);
        }

        [TestMethod()]
        public void ChangeRefValueTest()
        {
            bool called = false;
            PropertyChangedEventArgs epc = null;
            ObjectTest tst = new ObjectTest();
            tst.PropertyChanged += (s, e) => { called = true; epc = e; };
            tst.SValue = "Hello";

            Assert.IsTrue(called);
            Assert.AreEqual("SValue", epc.PropertyName);
            Assert.AreEqual("Hello", tst.SValue);
        }

    }
}

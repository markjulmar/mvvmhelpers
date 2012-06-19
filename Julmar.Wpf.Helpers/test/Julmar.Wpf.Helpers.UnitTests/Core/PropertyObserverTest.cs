using System.ComponentModel;
using JulMar.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using JulMar.Windows.Mvvm;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for PropertyObserverTest and is intended
    ///to contain all PropertyObserverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PropertyObserverTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        class TestObject : SimpleViewModel
        {
            private string _stest;
            public string SValue
            {
                get { return _stest; }
                set { _stest = value; RaisePropertyChanged("SValue"); }
            }

            private int _value;
            public int IValue
            {
                get { return _value; }
                set { _value = value; RaisePropertyChanged("IValue"); }
            }

            internal void ForceAllPropertyChange()
            {
                RaiseAllPropertiesChanged();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPropertyTest()
        {
            new PropertyObserver<TestObject>(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterHandlerNullTest()
        {
            TestObject obj = new TestObject {SValue = "Test String", IValue = 10};
            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.RegisterHandler(null, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterHandlerNullTest2()
        {
            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.RegisterHandler(o => o.SValue, null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UnregisterHandlerNullTest()
        {
            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.UnregisterHandler(null);
        }

        [TestMethod]
        public void NoTargetsTest()
        {
            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            obj.IValue = 5;
        }

        [TestMethod()]
        public void RegisterHandlerTest()
        {
            int actualInt = 0;
            string actualValue = null;

            TestObject obj = new TestObject {SValue = "Test String", IValue = 10};

            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.RegisterHandler(o => o.SValue, (oo) => actualValue = oo.SValue);
            po.RegisterHandler(o => o.IValue, (oo) => actualInt = oo.IValue);

            obj.SValue = "1";
            Assert.AreEqual("1", actualValue);
            obj.IValue = 5;
            Assert.AreEqual(5, actualInt);

            po.UnregisterHandler(o => o.SValue);
            obj.SValue = "2";
            Assert.AreEqual("1", actualValue);
        }

        [TestMethod()]
        public void UnregisterHandlerTest()
        {
            string actualValue = null;

            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            WeakReference wr = new WeakReference(obj);

            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.RegisterHandler(o => o.SValue, (oo) => actualValue = oo.SValue);

            obj.SValue = "1";
            Assert.AreEqual("1", actualValue);

            po.UnregisterHandler(o => o.SValue);
            obj.SValue = "2";
            Assert.AreEqual("1", actualValue);
        }

        [TestMethod()]
        public void RegisterAllPropsHandler()
        {
            bool calledPropChange = false;
            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.RegisterHandler(o => o.SValue, (oo) => calledPropChange = true);

            obj.ForceAllPropertyChange();
            Assert.IsTrue(calledPropChange);
        }

        [TestMethod()]
        public void DisposeTest()
        {
            string actualValue = null;

            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            WeakReference wr = new WeakReference(obj);

            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.RegisterHandler(o => o.SValue, (oo) => actualValue = oo.SValue);

            obj.SValue = "1";
            Assert.AreEqual("1", actualValue);

            po.Dispose();
            obj.SValue = "2";
            Assert.AreEqual("1", actualValue);

#if !DEBUG
            obj = null;
            GC.Collect(2,GCCollectionMode.Forced);
            Assert.IsFalse(wr.IsAlive);
#endif
        }

        [TestMethod()]
        public void RegisterGlobalHandler()
        {
            bool gotPropChange = false;
            string propName = null;
            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.PropertyChanged += (s, e) => { gotPropChange = true; propName = e.PropertyName; };

            obj.IValue = 5;
            Assert.IsTrue(gotPropChange);
            Assert.AreEqual("IValue", propName);
        }

        [TestMethod()]
        public void UnregisterGlobalHandler()
        {
            bool gotPropChange = false;
            string propName = null;
            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            var handler = new EventHandler<PropertyChangedEventArgs>((s, e) => { gotPropChange = true; propName = e.PropertyName; });
            po.PropertyChanged += handler;

            obj.IValue = 5;
            Assert.IsTrue(gotPropChange);

            gotPropChange = false;
            po.PropertyChanged -= handler;
            obj.IValue = 10;
            Assert.IsFalse(gotPropChange);
        }

        [TestMethod()]
        public void DisposeWithGlobalHandler()
        {
            bool gotPropChange = false;
            TestObject obj = new TestObject { SValue = "Test String", IValue = 10 };
            PropertyObserver<TestObject> po = new PropertyObserver<TestObject>(obj);
            po.PropertyChanged += (s, e) => { gotPropChange = true; };

            po.Dispose();
            obj.IValue = 5;
            Assert.IsFalse(gotPropChange);
        }
    }
}

using System;
using JulMar.Core;
using JulMar.Core.Interfaces;
using JulMar.Core.Services;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Composition;

namespace JulMar.Wpf.Helpers.UnitTests.Core
{
    /// <summary>
    ///This is a test class for MessageMediatorTest and is intended
    ///to contain all MessageMediatorTest Unit Tests
    ///</summary>
    [TestClass]
    public class MessageMediatorTest
    {
        public TestContext TestContext { get; set; }

        #region Inner Classes
        public interface IDataEvent
        {
            string SData { get; set; }
        }

        public class DataEvent : IDataEvent
        {
            public string SData { get; set; }
            public DataEvent(string s) { SData = s; }
        }

        public class DataEventUpper : IDataEvent
        {
            public string SData { get; set; }
            public DataEventUpper(string s) { SData = s.ToUpper(); }
        }

        public class DataEventLower : IDataEvent
        {
            public string SData { get; set; }
            public DataEventLower(string s) { SData = s.ToLower(); }
        }

        public interface ITest
        {
            void Message(IDataEvent data);
        }

        public class TestImpl : ITest
        {
            public int Count { get; set; }

            [MessageMediatorTarget]
            public void Message2(IDataEvent data) { Count += 1; }

            [MessageMediatorTarget("test")]
            public void Message(IDataEvent data) { Count += 2; }

            [MessageMediatorTarget]
            public void Message(DataEvent data) { Count += 4; }
        }

        public class TestImplJustInterfaces : ITest
        {
            public int Count { get; set; }

            [MessageMediatorTarget]
            public void Message(IDataEvent data) { Count += 1; }
        }

        class TestCounter
        {
            public int Count;

            [MessageMediatorTarget("test")]
            private void MessageHandler(object parameter)
            {
                if (((string)parameter) == "Test2")
                    ++Count;
            }

            [MessageMediatorTarget("test2")]
            public void MessageHandler2(object parameter)
            {
                if (((string)parameter) == "Test2")
                    ++Count;
            }

            [MessageMediatorTarget]
            private void MessageHandler3(string parameter)
            {
                if (parameter == "Test2")
                    ++Count;
            }

            [MessageMediatorTarget("test3")]
            internal void MessageHandler4(string parameter)
            {
                if (parameter == "Test2")
                    ++Count;
            }

            public void MessageHandler5(int parameter)
            {
                Count += parameter;
            }

            public void MessageHandler6(int parameter)
            {
                Count += parameter;
            }

            [MessageMediatorTarget("test7")]
            private void MessageHandler7(int parameter)
            {
                Count += parameter;
            }

            public void MessageHandler8(object parameter)
            {
                Count += (int)parameter;
            }
        }
        #endregion

        /// <summary>
        /// Test a single registration
        ///</summary>
        [TestMethod]
        public void RegisterSingleHandler()
        {
            var target = new MessageMediator();
            bool passedTest = false;

            Action<string> handler = o => passedTest = (o == "Test");
            target.RegisterHandler("test", handler);
            bool hadTarget = target.SendMessage("test", "Test");

            Assert.IsTrue(hadTarget, "Mediator did not return success");
            Assert.IsTrue(passedTest, "Did not receive message");

            target.UnregisterHandler("test", handler);
            Assert.IsFalse(target.SendMessage("test", "Test"));
        }

        /// <summary>
        /// Test no registrations
        ///</summary>
        [TestMethod]
        public void NoRegisterTest()
        {
            var target = new MessageMediator();
            bool passedTest = target.SendMessage("test", "Test");
            Assert.IsFalse(passedTest, "Mediator located unregistered target?");
        }

        /// <summary>
        /// Test type registration
        ///</summary>
        [TestMethod]
        public void RegisterInstanceTest()
        {
            var target = new MessageMediator();
            TestCounter counter = new TestCounter();
            target.Register(counter);
            bool hadTarget = target.SendMessage("test2", "Test2");

            Assert.IsTrue(hadTarget, "Mediator did not return success");
            Assert.AreEqual(1, counter.Count, "Method did not receive message");

            target.Unregister(counter);
            Assert.IsFalse(target.SendMessage("test2", "Test2"));
        }

        /// <summary>
        /// Test type registration
        ///</summary>
        [TestMethod]
        public void RegisterInstanceTest2()
        {
            var target = new MessageMediator();
            TestCounter counter = new TestCounter();
            target.Register(counter);

            bool hadTarget = target.SendMessage("test2", "Test2");
            Assert.IsTrue(hadTarget, "Mediator did not return success");

            hadTarget = target.SendMessage("test", "Test2");
            Assert.IsTrue(hadTarget, "Mediator did not return success");

            Assert.AreEqual(2, counter.Count, "Method did not receive message");

            target.Unregister(counter);
            Assert.IsFalse(target.SendMessage("test", "Test2"));
        }

        /// <summary>
        /// Unregister test
        ///</summary>
        [TestMethod]
        public void UnregisterInstanceTest()
        {
            var target = new MessageMediator();
            TestCounter counter = new TestCounter();
            target.Register(counter);
            bool hadTarget = target.SendMessage("test2", "Test2");

            Assert.IsTrue(hadTarget, "Mediator did not return success");
            Assert.AreEqual(1, counter.Count, "Method did not receive message");

            target.Unregister(counter);
            hadTarget = target.SendMessage("test2", "Test2");

            Assert.IsFalse(hadTarget, "Mediator did not return success");
            Assert.AreEqual(1, counter.Count, "Method did not receive message");
        }

        /// <summary>
        /// Unregister test
        ///</summary>
        [TestMethod]
        public void DeadInstanceTest()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();
            WeakReference wr = new WeakReference(tc);
            target.Register(tc);
            tc = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // If it's still alive, might be debug
            if (wr.IsAlive)
            {
                Assert.Inconclusive("Target still alive - debug mode?");
                tc = (TestCounter)wr.Target;
                if (tc != null)
                {
                    target.Unregister(wr.Target);
                    return;
                }
            }

            bool hadTarget = target.SendMessage("test2", "Test2");
            Assert.IsFalse(hadTarget, "Mediator located dead reference - debug mode?");
        }

        [TestMethod]
        public void TypeTest()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();
            target.Register(tc);

            bool hadTarget = target.SendMessage("Test2");
            Assert.IsTrue(hadTarget, "Mediator did not return success");
            hadTarget = target.SendMessage("test3", "Test2");
            Assert.IsTrue(hadTarget, "Mediator did not return success");
            Assert.AreEqual(2, tc.Count);

            target.Unregister(tc);
            Assert.IsFalse(target.SendMessage("test3", "Test2"));
        }

        [TestMethod]
        public void TestIntParam()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();
            target.Register(tc);

            bool hadTarget = target.SendMessage(5);
            Assert.IsFalse(hadTarget, "Mediator found unexpected target");

            target.RegisterHandler<int>(tc.MessageHandler5);
            hadTarget = target.SendMessage(5);
            Assert.IsTrue(hadTarget, "Mediator did not find target");
            Assert.AreEqual(5, tc.Count);

            target.Unregister(tc);
            target.UnregisterHandler<int>(tc.MessageHandler5);
            Assert.IsFalse(target.SendMessage(5));
        }

        [TestMethod]
        public void TestIntParam2()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();
            target.Register(tc);

            target.RegisterHandler<int>("test6", tc.MessageHandler6);
            bool hadTarget = target.SendMessage(5);
            Assert.IsFalse(hadTarget, "Mediator found unexpected target");

            hadTarget = target.SendMessage("test6", 5);
            Assert.IsTrue(hadTarget, "Mediator did not find target");
            Assert.AreEqual(5, tc.Count);

            target.UnregisterHandler<int>("test6", tc.MessageHandler6);
            target.Unregister(tc);
            Assert.IsFalse(target.SendMessage("test6", 5));
        }

        [TestMethod]
        public void TestIntParam3()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();

            target.Register(tc);
            bool hadTarget = target.SendMessage(5);
            Assert.IsFalse(hadTarget, "Mediator found unexpected target");

            hadTarget = target.SendMessage("test7", 1);
            Assert.IsTrue(hadTarget, "Mediator did not find expected target");

            target.RegisterHandler<int>("test7", tc.MessageHandler6);
            hadTarget = target.SendMessage("test7", 1);
            Assert.IsTrue(hadTarget, "Mediator did not find target");
            Assert.AreEqual(3, tc.Count);

            target.UnregisterHandler<int>("test7", tc.MessageHandler6);
            target.Unregister(tc);
            Assert.IsFalse(target.SendMessage("test7", 1));
        }

        [TestMethod]
        public void TestBadRegister()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();

            target.Register(tc);
            Assert.ThrowsException<ArgumentException>(() => target.RegisterHandler<int>("test", tc.MessageHandler5));
            target.Unregister(tc);
        }

        [TestMethod]
        public void TestInterfaceUsingDirectKey()
        {
            var target = new MessageMediator();
            var tc = new TestImpl();

            target.Register(tc);

            bool hadTarget = target.SendMessage("test", new DataEvent("Test"));
            Assert.IsTrue(hadTarget, "Mediator did not find target");
            Assert.AreEqual(2, tc.Count);

            target.Unregister(tc);
            Assert.IsFalse(target.SendMessage("test", new DataEvent("Test")));
        }

        [TestMethod]
        public void TestInterface()
        {
            var target = new MessageMediator();
            var tc = new TestImpl();

            target.Register(tc);

            // Should invoke two methods - interface AND direct
            bool hadTarget = target.SendMessage<IDataEvent>(new DataEvent("Test"));
            Assert.IsTrue(hadTarget, "Mediator did not find target");
            Assert.AreEqual(5, tc.Count);

            target.Unregister(tc);
            Assert.IsFalse(target.SendMessage<IDataEvent>(new DataEvent("Test")));
        }

        [TestMethod]
        public void TestInterfaceUsingConcreteClass()
        {
            var target = new MessageMediator();
            var tc = new TestImpl();

            target.Register(tc);

            // Should invoke two methods - interface AND direct
            bool hadTarget = target.SendMessage(new DataEvent("Test"));
            Assert.IsTrue(hadTarget, "Mediator did not find target");
            Assert.AreEqual(5, tc.Count);

            target.Unregister(tc);
            Assert.IsFalse(target.SendMessage(new DataEvent("Test")));
        }
       
        [TestMethod]
        public void TestJustInterfaces()
        {
            var target = new MessageMediator();
            var tc = new TestImplJustInterfaces();

            target.Register(tc);

            bool hadTarget = target.SendMessage(new DataEvent("Test"));
            Assert.IsTrue(hadTarget, "Mediator did not find target");

            hadTarget = target.SendMessage(new DataEventUpper("Test"));
            Assert.IsTrue(hadTarget, "Mediator did not find target");

            hadTarget = target.SendMessage(new DataEventLower("Test"));
            Assert.IsTrue(hadTarget, "Mediator did not find target");

            target.Unregister(tc);
            Assert.IsFalse(target.SendMessage(new DataEvent("Test")));
            Assert.IsFalse(target.SendMessage(new DataEventUpper("Test")));
            Assert.IsFalse(target.SendMessage(new DataEventLower("Test")));
        }

        class ImportTestClass
        {
            [Import]
            public IMessageMediator Mediator { get; set; }

            public ImportTestClass()
            {
                DynamicComposer.Instance.Compose(this);
            }
        }

        [TestMethod]
        public void TestSingleInstance()
        {
            var target = new ImportTestClass();
            var other = DynamicComposer.Instance.GetExportedValue<IMessageMediator>();
            Assert.AreSame(target.Mediator, other);
        }

    }
}

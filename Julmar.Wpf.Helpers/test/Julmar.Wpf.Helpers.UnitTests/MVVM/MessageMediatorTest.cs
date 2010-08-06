using System;
using JulMar.Windows.Mvvm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Julmar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for MessageMediatorTest and is intended
    ///to contain all MessageMediatorTest Unit Tests
    ///</summary>
    [TestClass]
    public class MessageMediatorTest
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Test a single registration
        ///</summary>
        [TestMethod]
        public void SingleRegisterTest()
        {
            var target = new MessageMediator();
            bool passedTest = false;

            target.RegisterHandler("test", (string o) => passedTest = (o == "Test"));
            bool hadTarget = target.SendMessage("test", "Test");

            Assert.AreEqual(true, hadTarget, "Mediator did not return success");
            Assert.AreEqual(true, passedTest, "Did not receive message");
        }

        /// <summary>
        /// Test no registrations
        ///</summary>
        [TestMethod]
        public void NoRegisterTest()
        {
            var target = new MessageMediator();
            bool passedTest = target.SendMessage("test", "Test");
            Assert.AreEqual(false, passedTest, "Mediator returned success");
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

            Assert.AreEqual(true, hadTarget, "Mediator did not return success");
            Assert.AreEqual(1, counter.Count, "Method did not receive message");
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
            Assert.AreEqual(true, hadTarget, "Mediator did not return success");

            hadTarget = target.SendMessage("test", "Test2");
            Assert.AreEqual(true, hadTarget, "Mediator did not return success");

            Assert.AreEqual(2, counter.Count, "Method did not receive message");
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

            Assert.AreEqual(true, hadTarget, "Mediator did not return success");
            Assert.AreEqual(1, counter.Count, "Method did not receive message");

            target.Unregister(counter);
            hadTarget = target.SendMessage("test2", "Test2");

            Assert.AreEqual(false, hadTarget, "Mediator did not return success");
            Assert.AreEqual(1, counter.Count, "Method did not receive message");
        }

        /// <summary>
        /// Unregister test
        ///</summary>
        [TestMethod]
        public void DeadInstanceTest()
        {
            var target = new MessageMediator();

            target.Register(new TestCounter());
            GC.Collect();

            bool hadTarget = target.SendMessage("test2", "Test2");
            Assert.AreEqual(true, hadTarget, "Mediator did not return success");
        }

        [TestMethod]
        public void TypeTest()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();
            target.Register(tc);

            bool hadTarget = target.SendMessage("Test2");
            Assert.AreEqual(true, hadTarget, "Mediator did not return success");
            hadTarget = target.SendMessage("test3", "Test2");
            Assert.AreEqual(true, hadTarget, "Mediator did not return success");

            Assert.AreEqual<int>(2, tc.Count);
        }

        [TestMethod]
        public void TestIntParam()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();
            target.Register(tc);

            bool hadTarget = target.SendMessage(5);
            Assert.AreEqual(false, hadTarget, "Mediator found unexpected target");

            target.RegisterHandler<int>(tc.MessageHandler5);
            hadTarget = target.SendMessage(5);
            Assert.AreEqual(true, hadTarget, "Mediator did not find target");
            Assert.AreEqual<int>(5, tc.Count);
        }

        [TestMethod]
        public void TestIntParam2()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();
            target.Register(tc);

            target.RegisterHandler<int>("test6", tc.MessageHandler6);
            bool hadTarget = target.SendMessage(5);
            Assert.AreEqual(false, hadTarget, "Mediator found unexpected target");

            hadTarget = target.SendMessage("test6", 5);
            Assert.AreEqual(true, hadTarget, "Mediator did not find target");
            Assert.AreEqual<int>(5, tc.Count);
        }

        [TestMethod]
        public void TestIntParam3()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();

            target.Register(tc);
            bool hadTarget = target.SendMessage(5);
            Assert.AreEqual(false, hadTarget, "Mediator found unexpected target");

            hadTarget = target.SendMessage("test7", 1);
            Assert.AreEqual(true, hadTarget, "Mediator did not find expected target");

            target.RegisterHandler<int>("test7", tc.MessageHandler6);
            hadTarget = target.SendMessage("test7", 1);
            Assert.AreEqual(true, hadTarget, "Mediator did not find target");
            Assert.AreEqual<int>(3, tc.Count);
        }

        [TestMethod]
        public void TestBadRegister()
        {
            var target = new MessageMediator();
            var tc = new TestCounter();

            target.Register(tc);

            try
            {
                target.RegisterHandler<int>("test", tc.MessageHandler5);
                Assert.Fail("Did not throw exception on invalid register");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestInterface()
        {
            var target = new MessageMediator();
            var tc = new Test3();

            target.Register(tc);

            bool hadTarget = target.SendMessage("test", new Data("Test"));
            Assert.AreEqual(true, hadTarget, "Mediator did not find target");
            Assert.AreEqual<int>(2, tc.Count);
        }

        [TestMethod]
        public void TestInterface2()
        {
            var target = new MessageMediator();
            var tc = new Test3();

            target.Register(tc);

            bool hadTarget = target.SendMessage<IData>(new Data("Test"));
            Assert.AreEqual(true, hadTarget, "Mediator did not find target");
            Assert.AreEqual<int>(1, tc.Count);
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

        public interface IData
        {
            string SData { get; set; }
        }

        public class Data : IData
        {
            public string SData { get; set; }
            public Data (string s)
            {
                SData = s;
            }
        }

        public interface ITest
        {
            void Message(IData data);
        }

        public class Test3 : ITest
        {
            public int Count;

            [MessageMediatorTarget("test")]
            public void Message(IData data)
            {
                Count += 2;
            }

            [MessageMediatorTarget]
            public void Message2(IData data)
            {
                Count += 1;
            }
        }

    }
}

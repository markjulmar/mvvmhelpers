using System.Collections.Generic;
using ServiceReplacement.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.Threading;

namespace ServiceReplacementTests
{
    /// <summary>
    ///This is a test class for MainViewModelTest and is intended
    ///to contain all MainViewModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MainViewModelTest
    {
        public class MockNotificationVisualizer : INotificationVisualizer, IDisposable
        {
            public DateTime? LastBeginCall { get; set; }
            public DateTime? LastDisposeCall { get; set; }
            readonly ManualResetEventSlim _waitEvent = new ManualResetEventSlim(false);

            public IDisposable BeginWait(string title, string message)
            {
                LastBeginCall = DateTime.Now;
                return this;
            }

            public void Dispose()
            {
                LastDisposeCall = DateTime.Now;
                _waitEvent.Set();
            }

            public bool WaitForDispose(TimeSpan waitTime, MockSynchronizationContext context)
            {
                DateTime start = DateTime.Now;
                while (DateTime.Now - start < waitTime)
                {
                    if (_waitEvent.Wait(1))
                        return true;
                    context.ExecutePostedCallbacks();
                }

                return false;
            }
        }

        public class MockMessageVisualizer : IMessageVisualizer
        {
            public object Response { get; set; }
            public void Show(string title, string message)
            {
            }

            public object Show(string title, string message, MessageVisualizerOptions visualizerOptions)
            {
                return Response;
            }
        }

        public class MockSynchronizationContext : SynchronizationContext
        {
            readonly List<Tuple<SendOrPostCallback,object>> _postedCallbacks = new List<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                d(state);
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (_postedCallbacks)
                {
                    _postedCallbacks.Add(Tuple.Create(d, state));
                }
            }

            public void ExecutePostedCallbacks()
            {
                lock (_postedCallbacks)
                {
                    _postedCallbacks.ForEach(t => t.Item1(t.Item2));
                    _postedCallbacks.Clear();
                }
            }
        }

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

        /// <summary>
        ///A test for CalculatePi
        ///</summary>
        [TestMethod()]
        public void CalculatePiTest()
        {
            MainViewModel_Accessor target = new MainViewModel_Accessor();

            MockNotificationVisualizer notifyVisualizer = new MockNotificationVisualizer();
            MockMessageVisualizer messageVisualizer = new MockMessageVisualizer { Response = 0 };
            
            ViewModel.ServiceProvider.Add(typeof(INotificationVisualizer), notifyVisualizer);
            ViewModel.ServiceProvider.Add(typeof(IMessageVisualizer), messageVisualizer);

            SynchronizationContext savedContext = SynchronizationContext.Current;
            MockSynchronizationContext mockContext = new MockSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(mockContext);

            target.CalculatePi.Execute(null);
            Assert.IsTrue(notifyVisualizer.WaitForDispose(TimeSpan.FromSeconds(10), mockContext));
            Assert.AreEqual(Math.PI.ToString(), target.PiText);

            Assert.IsTrue(notifyVisualizer.LastBeginCall != null);
            Assert.IsTrue(notifyVisualizer.LastDisposeCall != null);
            Assert.IsTrue(notifyVisualizer.LastDisposeCall > notifyVisualizer.LastBeginCall);

            ViewModel.ServiceProvider.Remove(typeof(INotificationVisualizer));
            ViewModel.ServiceProvider.Remove(typeof(IMessageVisualizer));
            SynchronizationContext.SetSynchronizationContext(savedContext);
        }

        /// <summary>
        ///A test for CalculatePi
        ///</summary>
        [TestMethod()]
        public void DoNot_CalculatePiTest()
        {
            MainViewModel_Accessor target = new MainViewModel_Accessor();

            MockNotificationVisualizer notifyVisualizer = new MockNotificationVisualizer();
            MockMessageVisualizer messageVisualizer = new MockMessageVisualizer { Response = 1 };

            ViewModel.ServiceProvider.Add(typeof(INotificationVisualizer), notifyVisualizer);
            ViewModel.ServiceProvider.Add(typeof(IMessageVisualizer), messageVisualizer);

            target.PiText = string.Empty;
            target.CalculatePi.Execute(null);
            Assert.AreEqual(string.Empty, target.PiText);

            Assert.IsTrue(notifyVisualizer.LastBeginCall == null);
            Assert.IsTrue(notifyVisualizer.LastDisposeCall == null);

            ViewModel.ServiceProvider.Remove(typeof(INotificationVisualizer));
            ViewModel.ServiceProvider.Remove(typeof(IMessageVisualizer));
        }

    }
}

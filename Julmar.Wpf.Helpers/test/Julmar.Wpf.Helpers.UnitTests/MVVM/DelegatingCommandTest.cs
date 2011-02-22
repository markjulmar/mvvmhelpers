using System;
using System.Threading;
using JulMar.Windows.Mvvm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Input;

namespace JulMar.Wpf.Helpers.UnitTests
{
    /// <summary>
    ///This is a test class for DelegatingCommandTest and is intended
    ///to contain all DelegatingCommandTest Unit Tests
    ///</summary>
    [TestClass]
    public class DelegatingCommandTest
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Test a single registration
        ///</summary>
        [TestMethod]
        public void ActionWithNoParamTest()
        {
            ICommand command = new DelegatingCommand(OnAction);

            Assert.AreEqual(true, command.CanExecute(null), "CanExecute did not return true");
            command.Execute(null);
            Assert.AreEqual(1, calledAction, "Command did not invoked OnAction");
        }

        /// <summary>
        /// Test a single registration
        ///</summary>
        [TestMethod]
        public void ActionWithParamTest()
        {
            ICommand command = new DelegatingCommand(OnAction2);

            Assert.AreEqual(true, command.CanExecute(5), "CanExecute did not return true");
            command.Execute(5);
            Assert.AreEqual(5, calledAction, "Command did not invoked OnAction2");
        }

        /// <summary>
        /// Test CanExecute logic
        ///</summary>
        [TestMethod]
        public void CanExecuteTest()
        {
            ICommand command = new DelegatingCommand(OnAction, () => false);
            Assert.AreEqual(false, command.CanExecute(null), "CanExecute did not return false");
        }

        /// <summary>
        /// Test CanExecute logic
        ///</summary>
        [TestMethod]
        public void CanExecuteTestWithParam()
        {
            ICommand command = new DelegatingCommand(OnAction2, (o) => !((bool)o));
            Assert.AreEqual(false, command.CanExecute(true), "CanExecute did not return false");
        }

        private int calledAction;
        void OnAction()
        {
            calledAction = 1;
        }

        void OnAction2(object parameter)
        {
            calledAction = (int) parameter;
        }

        class CommandListener
        {
            private Action work;
            public CommandListener(Action work)
            {
                this.work = work;
            }

            public void OnCanExecuteChanged(object sender, EventArgs e)
            {
                work();
            }
        }

        class MyTest
        {
            public int ExecuteCalls = 0;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void DoWork(object parameter)
            {
                ExecuteCalls++;
            }
        }

        [TestMethod]
        public void TestNoCommandManager()
        {
            int CanExecuteCalls = 0;
            CommandListener listener = new CommandListener(() => CanExecuteCalls++);
            MyTest test = new MyTest();
            DelegatingCommand command = new DelegatingCommand(test.DoWork, test.CanExecute, false);
            command.CanExecuteChanged += listener.OnCanExecuteChanged;

            CommandManager.InvalidateRequerySuggested();
            DispatcherUtil.DoEvents();
            command.Execute(null);
            Assert.AreEqual(1, test.ExecuteCalls);
            Assert.AreEqual(0, CanExecuteCalls);

            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, CanExecuteCalls);
        }

        [TestMethod]
        public void TestCommandManager()
        {
            int CanExecuteCalls = 0;
            CommandListener listener = new CommandListener(() => CanExecuteCalls++);
            MyTest test = new MyTest();
            DelegatingCommand command = new DelegatingCommand(test.DoWork, test.CanExecute, true);
            command.CanExecuteChanged += listener.OnCanExecuteChanged;

            command.Execute(null);
            DispatcherUtil.DoEvents();
            Assert.AreEqual(1, test.ExecuteCalls);
            Assert.AreEqual(0, CanExecuteCalls);

            CommandManager.InvalidateRequerySuggested();
            DispatcherUtil.DoEvents();
            Assert.AreEqual(1, test.ExecuteCalls);
            Assert.AreEqual(1, CanExecuteCalls);
        }

        [TestMethod]
        public void TestSwapToCommandManager()
        {
            int CanExecuteCalls = 0;
            CommandListener listener = new CommandListener(() => Interlocked.Increment(ref CanExecuteCalls));
            MyTest test = new MyTest();
            DelegatingCommand command = new DelegatingCommand(test.DoWork, test.CanExecute, false);

            command.CanExecuteChanged += listener.OnCanExecuteChanged;
            Assert.AreEqual(0, CanExecuteCalls);

            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, CanExecuteCalls);

            CommandManager.InvalidateRequerySuggested();
            DispatcherUtil.DoEvents();
            Assert.AreEqual(1, CanExecuteCalls);

            command.RaiseCanExecuteChanged();
            Assert.AreEqual(2, CanExecuteCalls);

            CommandManager.InvalidateRequerySuggested();
            DispatcherUtil.DoEvents();
            Assert.AreEqual(2, CanExecuteCalls);

            command.AutoCanExecuteRequery = true;
            CommandManager.InvalidateRequerySuggested();
            DispatcherUtil.DoEvents();
            Assert.AreEqual(3, CanExecuteCalls);

            command.RaiseCanExecuteChanged();
            Assert.AreEqual(4, CanExecuteCalls);

            command.AutoCanExecuteRequery = false;
            CommandManager.InvalidateRequerySuggested();
            DispatcherUtil.DoEvents();
            Assert.AreEqual(4, CanExecuteCalls);
        }

    }
}

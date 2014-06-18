using System;
using System.Windows.Input;
using JulMar.Mvvm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JulMar.Tests.MVVM
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
            ICommand command = new DelegateCommand(OnAction);

            Assert.AreEqual(true, command.CanExecute(null), "CanExecute did not return true");
            command.Execute(null);
            Assert.AreEqual(1, this.calledAction, "Command did not invoked OnAction");
        }

        /// <summary>
        /// Test a single registration
        ///</summary>
        [TestMethod]
        public void ActionWithParamTest()
        {
            ICommand command = new DelegateCommand(OnAction2);

            Assert.AreEqual(true, command.CanExecute(5), "CanExecute did not return true");
            command.Execute(5);
            Assert.AreEqual(5, this.calledAction, "Command did not invoked OnAction2");
        }

        /// <summary>
        /// Test CanExecute logic
        ///</summary>
        [TestMethod]
        public void CanExecuteTest()
        {
            ICommand command = new DelegateCommand(OnAction, () => false);
            Assert.AreEqual(false, command.CanExecute(null), "CanExecute did not return false");
        }

        /// <summary>
        /// Test CanExecute logic
        ///</summary>
        [TestMethod]
        public void CanExecuteTestWithParam()
        {
            ICommand command = new DelegateCommand(OnAction2, (o) => !((bool)o));
            Assert.AreEqual(false, command.CanExecute(true), "CanExecute did not return false");
        }

        private int calledAction;
        void OnAction()
        {
            this.calledAction = 1;
        }

        void OnAction2(object parameter)
        {
            this.calledAction = (int) parameter;
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
                this.work();
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
                this.ExecuteCalls++;
            }
        }
    }
}

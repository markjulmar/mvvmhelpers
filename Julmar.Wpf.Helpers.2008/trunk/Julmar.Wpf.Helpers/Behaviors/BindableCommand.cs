using System;
using System.Windows;
using System.Windows.Input;

namespace JulMar.Windows.Behaviors
{
    /// <summary>
    /// This class can be used to create a bound ICommand in resources
    /// which may then be assigned to the Command property of Key/Mouse gestures.
    /// </summary>
    public class BindableCommand : Freezable, ICommand
    {
        /// <summary>
        /// ICommand implementation to bind to the input type.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(BindableCommand));

        /// <summary>
        /// Parameter for the ICommand
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(BindableCommand));

        /// <summary>
        /// Gets and sets the Command
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets and sets the CommandParameter
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// This is used to determine if the command validity has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { Command.CanExecuteChanged += value; }
            remove { Command.CanExecuteChanged -= value; }
        }

        /// <summary>
        /// Returns whether the command is valid
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return Command.CanExecute(CommandParameter);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            Command.Execute(CommandParameter);
        }

        /// <summary>
        /// Creates an instance of the object.
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindableCommand();
        }
    }
}

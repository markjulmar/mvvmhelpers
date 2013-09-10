using JulMar.Windows.Behaviors;
using Microsoft.Xaml.Interactivity;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This trigger action binds a command/command parameter for MVVM usage with 
    /// a Blend based trigger.  This can be used as an alternative to EventCommander.
    /// </summary>
    public class InvokeCommand : DependencyObject, IAction
    {
        /// <summary>
        /// ICommand to execute
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", 
            typeof(ICommand), typeof(InvokeCommand), new PropertyMetadata(null));

        /// <summary>
        /// Command parameter to pass to command execution
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", 
            typeof(object), typeof(InvokeCommand), new PropertyMetadata(null));

        /// <summary>
        /// Path for CommandParameter when using Trigger Parameter
        /// </summary>
        public static readonly DependencyProperty CommandParameterPathProperty = DependencyProperty.Register("CommandParameterPath",
            typeof(PropertyPath), typeof(InvokeCommand), new PropertyMetadata(null));

        /// <summary>
        /// Command to execute
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Command parameter
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Path for final command parameter
        /// </summary>
        public PropertyPath CommandParameterPath
        {
            get { return (PropertyPath)GetValue(CommandParameterPathProperty); }
            set { SetValue(CommandParameterPathProperty, value); }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The <see cref="T:System.Object"/> that is passed to the action by the behavior. Generally this is <seealso cref="P:Microsoft.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param><param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <remarks>
        /// An example of parameter usage is EventTriggerBehavior, which passes the EventArgs as a parameter to its actions.
        /// </remarks>
        /// <returns>
        /// Returns the result of the action.
        /// </returns>
        public object Execute(object sender, object parameter)
        {
            ICommand command = Command;

            // Use either our set parameter, or the passed parameter.
            object commandParameter = (base.ReadLocalValue(CommandParameterProperty) != DependencyProperty.UnsetValue)
                ? CommandParameter : parameter;

            // Reach in and get the specific property if we have a path specified
            if (commandParameter != null && CommandParameterPath != null)
            {
                Binding binding = new Binding { Source = commandParameter, Path = CommandParameterPath };
                NameScopeBinding nsb = new NameScopeBinding();
                BindingOperations.SetBinding(nsb, NameScopeBinding.SourceProperty, binding);
                commandParameter = nsb.Source;
            }

            if ((command != null) && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
                return true;
            }

            return false;
        }
    }
}

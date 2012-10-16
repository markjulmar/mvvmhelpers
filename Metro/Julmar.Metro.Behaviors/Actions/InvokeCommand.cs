using JulMar.Windows.Behaviors;
using System.Windows.Input;
using System.Windows.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This trigger action binds a command/command parameter for MVVM usage with 
    /// a Blend based trigger.  This can be used as an alternative to EventCommander.
    /// </summary>
    public class InvokeCommand : TriggerAction<FrameworkElement>
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
        /// This is called to execute the command when the trigger conditions are satisfied.
        /// </summary>
        /// <param name="parameter">parameter (not used)</param>
        protected override void Invoke(object parameter)
        {
            ICommand command = Command;
            //TODO: need to find a way to detect Binding here - BindingOperations.IsDataBound
            object commandParameter = CommandParameter ?? parameter;

            // Reach in and get the specific property
            if (commandParameter != null && CommandParameterPath != null)
            {
                Binding binding = new Binding {Source = commandParameter, Path = CommandParameterPath};
                NameScopeBinding nsb = new NameScopeBinding();
                BindingOperations.SetBinding(nsb, NameScopeBinding.SourceProperty, binding);
                commandParameter = nsb.Source;
            }

            if ((command != null) && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }
    }
}

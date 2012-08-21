using System.Windows.Input;
using JulMar.Windows.Interactivity;
using Windows.UI.Xaml;

namespace JulMar.Windows.Actions
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
        /// This is called to execute the command when the trigger conditions are satisfied.
        /// </summary>
        /// <param name="parameter">parameter (not used)</param>
        protected override void Invoke(object parameter)
        {
            ICommand command = Command;
            if ((command != null) && command.CanExecute(CommandParameter))
                command.Execute(CommandParameter);
        }
    }
}

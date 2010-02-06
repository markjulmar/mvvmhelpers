using System;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace JulMar.Windows.Behaviors
{
    /// <summary>
    /// This class is used to attach the Window lifetime events to ICommand implementations.  It allows a ViewModel to 
    /// hook into the lifetime of the view (when necessary) through simple XAML tags.  Supported events are
    /// Loaded, Activated, Deactivated and Closing/Closed.  For the Closing/Closed event, the CanExecute handler is invoked
    /// in response to the Closing event - if it returns true, then the Closed event is allowed and the Execute handler is
    /// called in response.
    /// </summary>
    /// <remarks>
    /// Note that this class is deprecated because the EventCommander can do everything this class does.
    /// </remarks>
    /// <example>
    /// <![CDATA[  <Window MvvmHelpers:LifetimeEvent.Close="{Binding CloseCommand}" />  ]]>
    /// </example>
    public static class LifetimeEvent
    {
        /// <summary>
        /// Dependency property which holds the ICommand for the Loaded event
        /// </summary>
        public static readonly DependencyProperty LoadedProperty =
            DependencyProperty.RegisterAttached("Loaded", typeof(ICommand), typeof(LifetimeEvent),
                                                new UIPropertyMetadata(null, OnLoadedEventInfoChanged));

        /// <summary>
        /// Dependency property which holds the ICommand for the Activated event
        /// </summary>
        public static readonly DependencyProperty ActivatedProperty =
            DependencyProperty.RegisterAttached("Activated", typeof(ICommand), typeof(LifetimeEvent),
                                                new UIPropertyMetadata(null, OnActivatedEventInfoChanged));

        /// <summary>
        /// Dependency property which holds the ICommand for the Deactivated event
        /// </summary>
        public static readonly DependencyProperty DeactivatedProperty =
            DependencyProperty.RegisterAttached("Deactivated", typeof(ICommand), typeof(LifetimeEvent),
                                                new UIPropertyMetadata(null, OnDeactivatedEventInfoChanged));

        /// <summary>
        /// Dependency property which holds the ICommand for the Close event
        /// </summary>
        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.RegisterAttached("Close", typeof(ICommand), typeof(LifetimeEvent),
                                                new UIPropertyMetadata(null, OnCloseEventInfoChanged));

        /// <summary>
        /// Parameter for the ICommand
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
                            "CommandParameter", typeof(object), typeof(LifetimeEvent),
                                new UIPropertyMetadata(null));

        /// <summary>
        /// Attached Property getter to retrieve the ICommand
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <returns>ICommand</returns>
        public static ICommand GetLoaded(DependencyObject source)
        {
            return (ICommand)source.GetValue(LoadedProperty);
        }

        /// <summary>
        /// Attached Property setter to change the ICommand
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <param name="command">ICommand</param>
        public static void SetLoaded(DependencyObject source, ICommand command)
        {
            source.SetValue(LoadedProperty, command);
        }

        /// <summary>
        /// Attached Property getter to retrieve the ICommand
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <returns>ICommand</returns>
        public static ICommand GetActivated(DependencyObject source)
        {
            return (ICommand)source.GetValue(ActivatedProperty);
        }

        /// <summary>
        /// Attached Property setter to change the ICommand
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <param name="command">ICommand</param>
        public static void SetActivated(DependencyObject source, ICommand command)
        {
            source.SetValue(ActivatedProperty, command);
        }

        /// <summary>
        /// Attached Property getter to retrieve the ICommand
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <returns>ICommand</returns>
        public static ICommand GetDeactivated(DependencyObject source)
        {
            return (ICommand)source.GetValue(DeactivatedProperty);
        }

        /// <summary>
        /// Attached Property setter to change the ICommand
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <param name="command">ICommand</param>
        public static void SetDeactivated(DependencyObject source, ICommand command)
        {
            source.SetValue(DeactivatedProperty, command);
        }

        /// <summary>
        /// Attached Property getter to retrieve the ICommand
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <returns>ICommand</returns>
        public static ICommand GetClose(DependencyObject source)
        {
            return (ICommand)source.GetValue(CloseProperty);
        }

        /// <summary>
        /// Attached Property setter to change the ICommand
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <param name="command">ICommand</param>
        public static void SetClose(DependencyObject source, ICommand command)
        {
            source.SetValue(CloseProperty, command);
        }

        /// <summary>
        /// This retrieves the CommandParameter used for the command.
        /// </summary>
        /// <param name="source">Dependency object</param>
        /// <returns>Command Parameter passed to ICommand</returns>
        public static object GetCommandParameter(DependencyObject source)
        {
            return source.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// This changes the CommandParameter used with this command.
        /// </summary>
        /// <param name="source">Dependency Object</param>
        /// <param name="value">New Value</param>
        public static void SetCommandParameter(DependencyObject source, object value)
        {
            source.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// This is the property changed handler for the Loaded property.
        /// </summary>
        /// <param name="sender">Dependency Object</param>
        /// <param name="e">EventArgs</param>
        private static void OnLoadedEventInfoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var win = sender as Window;
            if (win == null)
                throw new ArgumentException("LifetimeEvent can only be used on Window class.");

            win.Closed -= PerformCleanup;
            win.Loaded -= OnWindowLoaded;
            if (e.NewValue != null)
            {
                win.Loaded += OnWindowLoaded;
                win.Closed += PerformCleanup;

                // Workaround: depending on the properties of the element, it's possible the Loaded event was already raised
                // This happens when the View is created before the ViewModel is applied to the DataContext.  In this
                // case, raise the Loaded event as soon as we know about it.
                if (win.IsLoaded)
                    OnWindowLoaded(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// This is the handler for the Loaded event to raise the command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWindowLoaded(object sender, EventArgs e)
        {
            var dpo = (DependencyObject)sender;
            ICommand loadedCommand = GetLoaded(dpo);
            if (loadedCommand != null)
                loadedCommand.Execute(GetCommandParameter(dpo));
        }

        /// <summary>
        /// This is the property changed handler for the Activated property.
        /// </summary>
        /// <param name="sender">Dependency Object</param>
        /// <param name="e">EventArgs</param>
        private static void OnActivatedEventInfoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var win = sender as Window;
            if (win == null)
                throw new ArgumentException("LifetimeEvent can only be used on Window class.");

            win.Closed -= PerformCleanup;
            win.Activated -= OnWindowActivated;
            if (e.NewValue != null)
            {
                win.Activated += OnWindowActivated;
                win.Closed += PerformCleanup;
            }
        }

        /// <summary>
        /// This is the handler for the Activated event to raise the command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWindowActivated(object sender, EventArgs e)
        {
            var dpo = (DependencyObject)sender;
            ICommand activatedCommand = GetActivated(dpo);
            if (activatedCommand != null)
                activatedCommand.Execute(GetCommandParameter(dpo));
        }

        /// <summary>
        /// This is the property changed handler for the Deactivated property.
        /// </summary>
        /// <param name="sender">Dependency Object</param>
        /// <param name="e">EventArgs</param>
        private static void OnDeactivatedEventInfoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var win = sender as Window;
            if (win == null)
                throw new ArgumentException("LifetimeEvent can only be used on Window class.");

            win.Deactivated -= OnWindowDeactivated;
            if (e.NewValue != null)
                win.Deactivated += OnWindowDeactivated;
        }

        /// <summary>
        /// This is the handler for the Deactivated event to raise the command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWindowDeactivated(object sender, EventArgs e)
        {
            var dpo = (DependencyObject)sender;
            ICommand deactivatedCommand = GetDeactivated(dpo);
            if (deactivatedCommand != null)
                deactivatedCommand.Execute(GetCommandParameter(dpo));
        }

        /// <summary>
        /// This is the property changed handler for the Close property.
        /// </summary>
        /// <param name="sender">Dependency Object</param>
        /// <param name="e">EventArgs</param>
        private static void OnCloseEventInfoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var win = sender as Window;
            if (win == null)
                throw new ArgumentException("LifetimeEvent can only be used on Window class.");

            win.Closed -= PerformCleanup;
            win.Closing -= OnWindowClosing;
            win.Closed -= OnWindowClosed;

            if (e.NewValue != null)
            {
                win.Closing += OnWindowClosing;
                win.Closed += OnWindowClosed;
                win.Closed += PerformCleanup;
            }
        }

        /// <summary>
        /// This method is invoked when the Window.Closing event is raised.  It checks with the ICommand.CanExecute handler
        /// and cancels the event if the handler returns false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var dpo = (DependencyObject) sender;
            ICommand ic = GetClose(dpo);
            if (ic != null)
                e.Cancel = !ic.CanExecute(GetCommandParameter(dpo));
        }

        /// <summary>
        /// This method is invoked when the Window.Closed event is raised.  It executes the ICommand.Execute handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnWindowClosed(object sender, EventArgs e)
        {
            var dpo = (DependencyObject)sender;
            ICommand ic = GetClose(dpo);
            if (ic != null)
                ic.Execute(GetCommandParameter(dpo));
        }

        /// <summary>
        /// This method is used to perform cleanup on the Window.Closed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PerformCleanup(object sender, EventArgs e)
        {
            Window win = sender as Window;
            if (win != null)
            {
                win.Closed -= PerformCleanup;
                win.Closing -= OnWindowClosing;
                win.Closed -= OnWindowClosed;
                win.Activated -= OnWindowActivated;
                win.Deactivated -= OnWindowDeactivated;
                win.Loaded -= OnWindowLoaded;
            }
        }
    }
}
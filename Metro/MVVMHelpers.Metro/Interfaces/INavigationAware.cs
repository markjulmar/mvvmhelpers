using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// EventArgs for INavigationAware.OnNavigatingFrom
    /// </summary>
    public class NavigatingFromEventArgs
    {
        /// <summary>
        /// Navigation model
        /// </summary>
        public NavigationMode NavigationMode { get; private set; }

        /// <summary>
        /// Set to true to cancel navigation
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// True if this is being called due to a suspension event
        /// </summary>
        public bool IsSuspending { get; set; }

        /// <summary>
        /// The current page state
        /// </summary>
        public IDictionary<string,object> State { get; private set; }

        /// <summary>
        /// Internal constructor
        /// </summary>
        internal NavigatingFromEventArgs(NavigationMode navigationMode, bool cancel, IDictionary<string, object> stateDictionary)
        {
            NavigationMode = navigationMode;
            Cancel = cancel;
            State = stateDictionary;
        }

        /// <summary>
        /// Constructor used for suspension
        /// </summary>
        /// <param name="stateDictionary"></param>
        internal NavigatingFromEventArgs(IDictionary<string,object> stateDictionary)
        {
            NavigationMode = NavigationMode.Refresh; // dummy
            Cancel = false;
            State = stateDictionary;
            IsSuspending = true;
        }
    }

    /// <summary>
    /// EventArgs for the INavigationAware.OnNavigatedTo
    /// </summary>
    public class NavigatedToEventArgs
    {
        /// <summary>
        /// Navigation mode for this event
        /// </summary>
        public NavigationMode NavigationMode { get; private set; }

        /// <summary>
        /// Optional parameter being passed
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// ViewModel/Page state
        /// </summary>
        public IDictionary<string, object> State { get; private set; }

        /// <summary>
        /// Internal constructor
        /// </summary>
        internal NavigatedToEventArgs(NavigationMode navigationMode, object parameter, IDictionary<string, object> stateDictionary)
        {
            NavigationMode = navigationMode;
            Parameter = parameter;
            State = stateDictionary;
        }
    }

    /// <summary>
    /// This interface can be implemented by a ViewModel or Page to save/restore state
    /// during navigation.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Notification that the Page associated with the ViewModel is navigating away
        /// </summary>
        void OnNavigatingFrom(NavigatingFromEventArgs e);

        /// <summary>
        /// Notification that the Page associated with the ViewModel is being navigating to
        /// </summary>
        void OnNavigatedTo(NavigatedToEventArgs e);
    }
}
using System.ComponentModel;
using System.Diagnostics;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This class implements the simplest view model -- one that implements INPC.
    /// </summary>
    public class SimpleViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// This can be used to indicate that all property values have changed.
        /// </summary>
        protected void OnPropertyChanged()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// This raises the INotifyPropertyChanged.PropertyChanged event to indicate
        /// a specific property has changed value.
        /// </summary>
        /// <param name="name">Primary property</param>
        /// <param name="propertyNames">Additional properties</param>
        protected virtual void OnPropertyChanged(string name, params string[] propertyNames)
        {
            Debug.Assert(string.IsNullOrEmpty(name) || GetType().GetProperty(name) != null);
            PropertyChanged(this, new PropertyChangedEventArgs(name));
            if (propertyNames != null)
            {
                foreach (var propName in propertyNames)
                    OnPropertyChanged(propName);
            }
        }
    }
}

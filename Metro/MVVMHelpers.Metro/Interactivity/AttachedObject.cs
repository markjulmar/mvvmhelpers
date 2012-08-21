using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JulMar.Windows.Interactivity
{
    /// <summary>
    /// This class represents an object that may be attached to a FrameworkElement
    /// to extend or alter it's behavior.
    /// Modeled after System.Windows.Interactivity: remove once that is available
    /// </summary>
    public abstract class AttachedObject : FrameworkElement
    {
        private FrameworkElement _associatedObject;

        /// <summary>
        /// Type of the associated object
        /// </summary>
        public virtual Type AssociatedObjectType { get; private set; }

        /// <summary>
        /// Constructor to attach to a specific type of object
        /// </summary>
        /// <param name="associatedObjectType">Type of object to attach to</param>
        protected AttachedObject(Type associatedObjectType)
        {
            AssociatedObjectType = associatedObjectType;
        }

        /// <summary>
        /// Override called when behavior is attached
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Override called when behavior is detached
        /// </summary>
        protected virtual void OnDetaching()
        {
        }

        /// <summary>
        /// The associated object
        /// </summary>
        internal FrameworkElement AssociatedObjectInternal
        {
            get { return _associatedObject; }
            set
            {
                if (value != null)
                {
                    TypeInfo tiAllowed = AssociatedObjectType.GetTypeInfo();
                    if (!tiAllowed.IsAssignableFrom(value.GetType().GetTypeInfo()))
                    {
                        throw new ArgumentException( "Invalid argument type for the associated object", "value");
                    }
                }

                DetachExistingObject();
                AttachObject(value);
            }
        }

        /// <summary>
        /// This method attaches the behavior to a specific FrameworkElement and wires into 
        /// the events it needs to monitor layout and loading.
        /// </summary>
        /// <param name="value">FrameworkElement to attach to</param>
        private void AttachObject(FrameworkElement value)
        {
            if (value != null)
            {
                _associatedObject = value;
                _associatedObject.Unloaded += OnAssociatedObjectUnloaded;
                _associatedObject.LayoutUpdated += OnAssociatedObjectLayoutUpdated;

                OnAttached();
            }
        }

        /// <summary>
        /// Called when the associated object's Layout changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAssociatedObjectLayoutUpdated(object sender, object e)
        {
            if (_associatedObject != null)
            {
                _associatedObject.LayoutUpdated -= OnAssociatedObjectLayoutUpdated;
                SetBinding(DataContextProperty, new Binding { Source = _associatedObject, Path = new PropertyPath("DataContext"), Mode = BindingMode.OneWay });
            }
        }

        /// <summary>
        /// This detaches the behavior from it's associated object
        /// </summary>
        private void DetachExistingObject()
        {
            if (_associatedObject != null)
            {
                _associatedObject.LayoutUpdated -= OnAssociatedObjectLayoutUpdated;
                _associatedObject.Unloaded -= OnAssociatedObjectUnloaded;
                OnDetaching();
            }

            _associatedObject = null;
            ClearValue(DataContextProperty);
        }

        /// <summary>
        /// Called when the associated object is unloaded
        /// </summary>
        private void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObjectInternal = null;
        }
    }
}

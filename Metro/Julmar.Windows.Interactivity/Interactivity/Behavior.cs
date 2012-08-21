using Windows.UI.Xaml;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// An attachable behavior
    /// </summary>
    public abstract class Behavior : AttachedObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="associatedObjectType"></param>
        public Behavior(Type associatedObjectType) : base(associatedObjectType)
        {
        }
    }

    /// <summary>
    /// A typed attachable behavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Behavior<T> : Behavior where T : FrameworkElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Behavior() : base(typeof(T))
        {
        }

        /// <summary>
        /// Associated object
        /// </summary>
        public T AssociatedObject
        {
            set { AssociatedObjectInternal = value; }
            get { return (T) AssociatedObjectInternal; }
        }
    }
}

namespace JulMar.Core.Collections
{
    /// <summary>
    /// This is passed as the argument to an ElementChanged event from a ThreadedCollection.
    /// </summary>
    /// <typeparam name="T">Element Type</typeparam>
    public class ElementChangedEventArgs<T> : PropertyChangedEventArgsEx
    {
        /// <summary>
        /// Collection owner that changed [do not manipulate]
        /// </summary>
        public ThreadedCollection<T> Collection { get; private set; }

        /// <summary>
        /// Element that was changed
        /// </summary>
        public T Element { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">Collection container</param>
        /// <param name="element">Element</param>
        /// <param name="propName">Property name</param>
        public ElementChangedEventArgs(ThreadedCollection<T> container, T element, string propName) : base(propName)
        {
            Collection = container;
            Element = element;
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="container">Collection container</param>
        /// <param name="element">Element</param>
        /// <param name="propertyName">Property that has changed</param>
        /// <param name="newValue">New Value</param>
        public ElementChangedEventArgs(ThreadedCollection<T> container, T element, string propertyName, object newValue)
            : base(propertyName, newValue)
        {
            Collection = container;
            Element = element;
        }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="container">Collection container</param>
        /// <param name="element">Element</param>
        /// <param name="propertyName">Property that has changed</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New Value</param>
        public ElementChangedEventArgs(ThreadedCollection<T> container, T element, string propertyName, object oldValue, object newValue)
            : base(propertyName, oldValue, newValue)
        {
            Collection = container;
            Element = element;
        }
    }
}

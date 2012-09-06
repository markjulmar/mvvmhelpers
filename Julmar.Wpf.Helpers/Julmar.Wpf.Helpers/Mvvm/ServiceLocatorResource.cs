using System;
using System.Windows;

namespace JulMar.Core.Services
{
    /// <summary>
    /// This class allows an injected service location into resources for
    /// XAML based binding lookups
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///   <Page.Resources>
    ///      <ServiceLocatorResource Type="ViewModelLocator" x:Key="vmLocator" />
    ///   </Page.Resources>
    /// ]]>
    /// </example>
    public sealed class ServiceLocatorResource : DependencyObject
    {
        /// <summary>
        /// Backing storage for the type
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof (Type), typeof (ServiceLocatorResource), new PropertyMetadata(null));

        /// <summary>
        /// Constructor
        /// </summary>
        public ServiceLocatorResource()
        {
        }

        /// <summary>
        /// Code constructor
        /// </summary>
        /// <param name="type">Type to lookup</param>
        public ServiceLocatorResource(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Type to lookup
        /// </summary>
        public Type Type
        {
            get { return (Type) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        private object _instance;

        /// <summary>
        /// Located Instance
        /// </summary>
        public object Instance
        {
            get
            {
                if (_instance == null && Type != null)
                {
                    _instance = DynamicComposer.Instance.GetExportedValue(Type);
                }
                return _instance;
            }
        }
    }
}

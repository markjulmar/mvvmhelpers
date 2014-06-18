using System;
using System.Windows.Markup;

using JulMar.Extensions;
using JulMar.Interfaces;
using JulMar.Services;

namespace JulMar.Markup
{
    /// <summary>
    /// This provides a simple way to establish a ViewModel for a View through a markup extension
    /// </summary>
    public class ViewModelCreatorExtension : MarkupExtension
    {
        #region Public Properties
        /// <summary>
        /// View Model key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Type of the view model to create
        /// </summary>
        public Type ViewModelType { get; set; }
        #endregion

        /// <summary>
        /// Common constructor
        /// </summary>
        public ViewModelCreatorExtension()
        {
        }

        /// <summary>
        /// Constructor that takes a specific type
        /// </summary>
        /// <param name="runtimeType">Type to create</param>
        public ViewModelCreatorExtension(Type runtimeType) : this()
        {
            this.ViewModelType = runtimeType;
        }

        /// <summary>
        /// Returns an object to represent the ViewModel.
        /// </summary>
        /// <returns>
        /// The object value to set on the property where the extension is applied. 
        /// </returns>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            try
            {
                // Create the VM
                return this.CreateOrLocateViewModel();
            }
            catch
            {
                // If we are in design mode, then don't allow the exception to propagate out
                // It kills the design surface.
                if (Designer.InDesignMode)
                    return null;
                
                // Otherwise throw it
                throw;
            }
        }

        /// <summary>
        /// This either creates the VM directly using the Types supplied, or looks up the VM
        /// with MEF using the key and creates it that way.
        /// </summary>
        /// <returns></returns>
        private object CreateOrLocateViewModel()
        {
            if (!string.IsNullOrEmpty(this.Key))
            {
                object vm;
                if (ServiceLocater.Instance.Resolve<IViewModelLocater>().TryLocate(this.Key, out vm))
                    return vm;
            }

            return this.ViewModelType != null ? Activator.CreateInstance(this.ViewModelType) : null;
        }
    }
}
using System;
using System.Windows.Markup;
using JulMar.Windows.Mvvm;
using System.Windows;
using JulMar.Windows.Extensions;
using System.Diagnostics;

namespace JulMar.Windows.Markup
{
    /// <summary>
    /// This provides a simple way to establish a ViewModel for a View through a markup extension
    /// </summary>
    public class ViewModelCreatorExtension : MarkupExtension
    {
        // The window we are bound to
        private Window _winTarget;

        // The view model we created
        private object _viewModel;

        /// <summary>
        /// View Model key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Type of the view model to create
        /// </summary>
        public Type ViewModelType { get; set; }

        /// <summary>
        /// Designer view model type (if different than runtime)
        /// </summary>
        public Type DesignerViewModelType { get; set; }

        /// <summary>
        /// True to automatically dispose the VM when the view closes.
        /// Defaults to TRUE - set it to FALSE to turn this off.
        /// </summary>
        public bool DisposeOnClose { get; set; }

        /// <summary>
        /// Common constructor
        /// </summary>
        public ViewModelCreatorExtension()
        {
            DisposeOnClose = true;
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="runtimeType">Type to create</param>
        public ViewModelCreatorExtension(Type runtimeType)
            : this()
        {
            ViewModelType = runtimeType;
        }

        /// <summary>
        /// Constructor used when specifying both a runtime ViewModel and
        /// a Designer ViewModel.
        /// </summary>
        /// <param name="runtimeType">Runtime type</param>
        /// <param name="designerType">Designer type</param>
        public ViewModelCreatorExtension(Type runtimeType, Type designerType) : this(runtimeType)
        {
            DesignerViewModelType = designerType;
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
                _viewModel = CreateOrLocateViewModel();

                // If it's a ViewModel type, and the target is a top-level Window, then subscribe to the close request
                // event and close the window if the view model raises the event.
                ViewModel jvm = _viewModel as ViewModel;
                if (jvm != null)
                {
                    // Get the element we are hooking up.
                    IProvideValueTarget ipvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
                    if (ipvt != null)
                    {
                        _winTarget = ipvt.TargetObject as Window;
                        if (_winTarget != null)
                        {
                            jvm.ActivateRequest += ActivateViewRequest;
                            jvm.CloseRequest += CloseViewRequest;
                            _winTarget.Closed += WindowClosed;
                        }
                    }
                }

                return _viewModel;
            }
            catch
            {
                // If we are in design mode, then don't allow the exception to propogate out -- it kills the design surface.
                if (Designer.InDesignMode == true)
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
            if (!string.IsNullOrEmpty(Key))
            {
                var vm = ViewModel.ServiceProvider.Resolve<ViewModelLocator>().Locate(Key);
                if (vm != null)
                    return vm;
            }

            Type typeToCreate = ((Designer.InDesignMode) ? DesignerViewModelType : ViewModelType) ?? ViewModelType;
            return typeToCreate != null ? Activator.CreateInstance(typeToCreate) : null;
        }

        /// <summary>
        /// Event handler that is invoked when the window is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WindowClosed(object sender, EventArgs e)
        {
            Window winTarget = (Window)sender;
            Debug.Assert(winTarget == _winTarget);

            ViewModel jvm = _viewModel as ViewModel;
            if (jvm != null)
            {
                jvm.CloseRequest -= CloseViewRequest;
                jvm.ActivateRequest -= ActivateViewRequest;
                
                if (DisposeOnClose)
                    jvm.Dispose();
            }

            winTarget.Closed -= WindowClosed;
            _viewModel = null;
            _winTarget = null;
        }

        /// <summary>
        /// Event handler that is invoked when the ViewModel is
        /// requesting the view to close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CloseViewRequest(object sender, CloseRequestEventArgs e)
        {
            if (!_winTarget.Dispatcher.CheckAccess())
            {
                _winTarget.Dispatcher.Invoke((EventHandler<CloseRequestEventArgs>)CloseViewRequest, sender, e);
                return;
            }

            try
            {
                _winTarget.DialogResult = e.Result;
            }
            // Raised if this was displayed via Show() vs. ShowDialog
            catch (InvalidOperationException)
            {
                _winTarget.Close();
            }
        }

        /// <summary>
        /// Event handler called when the ViewModel is requesting to
        /// activate (surface) the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ActivateViewRequest(object sender, EventArgs e)
        {
            if (!_winTarget.Dispatcher.CheckAccess())
            {
                _winTarget.Dispatcher.Invoke((Action)(() => _winTarget.Activate()), null);
            }
            else _winTarget.Activate();
        }
    }
}
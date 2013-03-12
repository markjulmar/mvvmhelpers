using System.Composition;
using JulMar.Core.Services;
using JulMar.Windows.Interfaces;
using Windows.UI.Xaml;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// ViewModel locator resource lookup.  This provides markup support for the 
    /// IViewModelLocator service.  It can be used two different ways:
    ///   a) By placing an instance of the ViewModelLocator into Application.Resources in XAML and
    ///      then data binding to the resource with the [ViewModelKey] path (array indexer).
    ///   b) Through the ViewModelLocator.Key attached property which sets the 
    ///      DataContext of the element to which it is attached
    /// </summary>
    public sealed class ViewModelLocator
    {
        /// <summary>
        /// ViewModel key
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
                DependencyProperty.RegisterAttached("Key", typeof(string), 
                    typeof(ViewModelLocator), new PropertyMetadata(default(string), OnViewModelKeyChanged));

        /// <summary>
        /// ViewModel dictionary - can be used as indexer operator in Binding expressions.
        /// </summary>
        [Import]
        public IViewModelLocator ViewModels { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelLocator()
        {
            DynamicComposer.Instance.Compose(this);
        }

        /// <summary>
        /// Called when the ViewModelKey property is changed on an UI element.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnViewModelKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe != null)
            {
                string key = e.NewValue as string;
                if (!string.IsNullOrEmpty(key))
                {
                    IViewModelLocator vmLocator = ServiceLocator.Instance.Resolve<IViewModelLocator>();
                    object viewModel;
                    if (vmLocator.TryLocate(key, out viewModel) && viewModel != null)
                    {
                        fe.DataContext = viewModel;
                    }
                }
            }
        }

        /// <summary>
        /// Get the ViewModelKey property value
        /// </summary>
        /// <param name="fe"></param>
        /// <returns></returns>
        public static string GetKey(FrameworkElement fe)
        {
            return (string) fe.GetValue(KeyProperty);
        }

        /// <summary>
        /// Sets the ViewModelKey property value.
        /// </summary>
        /// <param name="fe"></param>
        /// <param name="key"></param>
        public static void SetKey(FrameworkElement fe, string key)
        {
            fe.SetValue(KeyProperty, key);
        }
    }
}
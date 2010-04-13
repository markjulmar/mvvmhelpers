using System;
using JulMar.Windows.Interfaces;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This is used to trigger a behavior on the View.
    /// </summary>
    public class ViewModelTrigger : IViewModelTrigger
    {
        /// <summary>
        /// Action - hooked by the View
        /// </summary>
        private Action<object> _func;

        /// <summary>
        /// Event used by the view.
        /// </summary>
        event Action<object> IViewModelTrigger.Execute
        {
            add { _func += value; }
            remove { _func -= value; }
        }

        ///<summary>
        /// This is called by the ViewModel to trigger some behavior
        /// in the view.
        ///</summary>
        ///<param name="parameter">Optional parameter</param>
        ///<returns>True if raised event</returns>
        public bool Execute(object parameter)
        {
            var function = _func;
            if (function != null)
            {
                function.Invoke(parameter);
                return true;
            }
            return false;
        }

        ///<summary>
        /// This is called by the ViewModel to trigger some behavior
        /// in the view.
        ///</summary>
        ///<returns>True if raised event</returns>
        public bool Execute()
        {
            var function = _func;
            if (function != null)
            {
                function.Invoke(null);
                return true;
            }
            return false;
        }
    }
}

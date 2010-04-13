using System;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// Interface used to drive actions from the VM -> View.
    /// </summary>
    public interface IViewModelTrigger
    {
        /// <summary>
        /// This event is used to trigger the action
        /// </summary>
        event Action<object> Execute;
    }
}

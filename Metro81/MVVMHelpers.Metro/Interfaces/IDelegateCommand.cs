using System.Windows.Input;

namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// An extension to ICommand to provide an ability to raise changed events.
    /// </summary>
    public interface IDelegateCommand : ICommand
    {
        /// <summary>
        /// This method can be used to raise the CanExecuteChanged handler.
        /// This will force WPF to re-query the status of this command directly.
        /// This is not necessary if you use the AutoCanExecuteRequery feature.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}

using JulMar.Core;
using JulMar.Windows.Mvvm;
using System.Windows.Input;
using JulMar.Windows.Interfaces;
using System.IO;

namespace MVVMFolderExplorer.ViewModels
{
    /// <summary>
    /// This is the main view model - created and associated to the MainView window.
    /// </summary>
    [ExportViewModel("MainWindow")]
    public class MainViewModel : ViewModel
    {
        #region Internal Data
        private DirectoryViewModel _selectedDirectory;
        #endregion

        /// <summary>
        /// Root directory - can be bound to an ItemsControl on the UI.
        /// </summary>
        public DirectoryViewModel[] RootDirectory { get; private set; }

        /// <summary>
        /// Selected (active) directory
        /// </summary>
        public DirectoryViewModel SelectedDirectory
        {
            get { return _selectedDirectory; }
            set { SetPropertyValue(ref _selectedDirectory, value, () => SelectedDirectory); }
        }

        /// <summary>
        /// Command to display the About Box.
        /// </summary>
        public ICommand DisplayAboutCommand { get; private set; }

        /// <summary>
        /// Command to end the application
        /// </summary>
        public ICommand CloseAppCommand { get; private set; }

        /// <summary>
        /// Main constructor
        /// </summary>
        public MainViewModel()
        {
            // Create our commands
            DisplayAboutCommand = new DelegateCommand(OnShowAbout);
            CloseAppCommand = new DelegateCommand(OnCloseApp);

            // Fill in the root directory from C:
            RootDirectory = new[] { new DirectoryViewModel(new DirectoryInfo(@"C:\")) { IsSelected = true } };
        }

        /// <summary>
        /// This method closes the application window.
        /// </summary>
        private void OnCloseApp()
        {
            // Ask the view to close.
            RaiseCloseRequest();
        }

        /// <summary>
        /// This method displays the About Box.
        /// </summary>
        private void OnShowAbout()
        {
            // Display a popup with our version using the message visualizer.
            Resolve<IMessageVisualizer>().Show("About File Explorer Sample", "File Explorer Sample 1.0");
        }

        /// <summary>
        /// This method is invoked by the message mediator when a DirectoryViewModel is selected.
        /// </summary>
        /// <param name="newDirectory">DirectoryViewModel that is now active</param>
        [MessageMediatorTarget(DirectoryViewModel.SelectedDirectoryChangedMessage)]
        private void OnCurrentDirectoryChanged(DirectoryViewModel newDirectory)
        {
            SelectedDirectory = newDirectory;
        }
    }
}

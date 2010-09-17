using JulMar.Windows.Mvvm;
using System.IO;

namespace MVVMFolderExplorer.ViewModels
{
    /// <summary>
    /// ViewModel that wraps a file.  Since this does not use any visualizations or services
    /// it extends from SimpleViewModel - this supports only property notification and is lighter weight
    /// than the full ViewModel base class.
    /// </summary>
    public class FileViewModel : SimpleViewModel
    {
        /// <summary>
        /// File expansion marker used by the directory
        /// </summary>
        internal static FileViewModel MarkerFile = new FileViewModel();

        #region Internal Data
        private readonly FileInfo _data;
        #endregion

        /// <summary>
        /// File name
        /// </summary>
        public string Name
        {
            get { return _data.Name; }
        }

        /// <summary>
        /// Full path + filename
        /// </summary>
        public string FullPath
        {
            get { return _data.FullName; }
        }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public long Size
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Internal constructor used to create the file marker.
        /// </summary>
        private FileViewModel()
        {
            _data = null;
        }

        /// <summary>
        /// Public constructor that captures a list of files.
        /// </summary>
        /// <param name="fi">FileInfo to grab file information from</param>
        public FileViewModel(FileInfo fi)
        {
            _data = fi;
        }
    }
}

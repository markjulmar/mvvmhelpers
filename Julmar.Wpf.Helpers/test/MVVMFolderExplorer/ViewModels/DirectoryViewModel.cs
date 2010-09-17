using System;
using System.Collections.ObjectModel;
using System.Linq;
using JulMar.Core.Extensions;
using JulMar.Windows.Mvvm;
using System.IO;
using System.Collections.Generic;

namespace MVVMFolderExplorer.ViewModels
{
    /// <summary>
    /// Sample ViewModel that wraps a Directory.
    /// </summary>
    public class DirectoryViewModel : ViewModel
    {
        /// <summary>
        /// String used to send message to main view model about directory selection.
        /// </summary>
        internal const string SelectedDirectoryChangedMessage = @"SelectedDirectoryChanged";

        /// <summary>
        /// Marker directory that signals expansion of the tree.
        /// </summary>
        internal static DirectoryViewModel MarkerDirectory = new DirectoryViewModel();

        #region Internal Data
        private bool _isSelected, _isExpanded;
        private readonly DirectoryInfo _data;
        private readonly ObservableCollection<FileViewModel> _files;
        private readonly ObservableCollection<DirectoryViewModel> _subdirs;
        #endregion

        /// <summary>
        /// Name of the directory
        /// </summary>
        public string Name
        {
            get { return _data.Name; }
        }

        /// <summary>
        /// Full path + name of the directory
        /// </summary>
        public string FullName
        {
            get { return _data.FullName; }
        }

        /// <summary>
        /// True/False whether the directory is selected (i.e. current).
        /// Selecting the directory causes it to populate it's file collection.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    if (_isSelected)
                    {
                        if (_files.Count == 1 && _files[0] == FileViewModel.MarkerFile)
                        {
                            _files.Clear();
                            _data.GetFiles()
                                .Where(f => (f.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                                .ForEach(f => _files.Add(new FileViewModel(f)));
                            OnPropertyChanged("TotalFiles", "TotalFileSize");
                        }
                        SendMessage(SelectedDirectoryChangedMessage, this);
                    }
                    else
                    {
                        _files.Clear();
                        _files.Add(FileViewModel.MarkerFile);
                    }

                    OnPropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// True/False if the directory is expanded. Expanding the directory causes it
        /// to fill it's subdirectory collection.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    if (_isExpanded)
                    {
                        if (_subdirs.Count == 1 && _subdirs[0] == DirectoryViewModel.MarkerDirectory)
                        {
                            _subdirs.Clear();
                            _data.GetDirectories()
                                .Where(d => (d.Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                                .ForEach(d => _subdirs.Add(new DirectoryViewModel(d)));
                        }
                    }
                    // Throw them away to recollect later - implements a refresh.
                    else
                    {
                        _subdirs.Clear();
                        _subdirs.Add(DirectoryViewModel.MarkerDirectory);
                    }
                }

                OnPropertyChanged("IsExpanded");
            }
        }

        /// <summary>
        /// List of files in this directory.
        /// </summary>
        public IList<FileViewModel> Files { get { return _files; } }

        /// <summary>
        /// List of subdirectories in this directory.
        /// </summary>
        public IList<DirectoryViewModel> Subdirectories { get { return _subdirs; } }

        /// <summary>
        /// Count of files in this directory.
        /// </summary>
        public int TotalFiles { get { return _files.Count; } }

        /// <summary>
        /// Total size of all files in this directory.
        /// </summary>
        public long TotalFileSize { get { return _files.Sum(file => file.Size); } }

        /// <summary>
        /// Constructor for the marker directory.  This is used to detect an expansion.
        /// </summary>
        private DirectoryViewModel()
        {
            _data = null;
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="di">DirectoryInfo to pull information from</param>
        public DirectoryViewModel(DirectoryInfo di)
        {
            if (di == null)
                throw new ArgumentNullException("di");

            _data = di;
            _files = new ObservableCollection<FileViewModel> { FileViewModel.MarkerFile };
            _subdirs = new ObservableCollection<DirectoryViewModel> { DirectoryViewModel.MarkerDirectory };
        }
    }
}

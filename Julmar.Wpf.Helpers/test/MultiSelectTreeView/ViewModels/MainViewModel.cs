using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MultiSelectTreeView.ViewModels
{
	public class MainViewModel
	{
        public IList<FolderViewModel> RootFolders { get; private set; }
        public IList<FolderViewModel> SelectedFolders { get; private set; }

	    public MainViewModel()
	    {
	        RootFolders = new ObservableCollection<FolderViewModel>();
            SelectedFolders = new ObservableCollection<FolderViewModel>();
            Fill(RootFolders, 1, 5);
	    }

        private void Fill(IList<FolderViewModel> folders, int currentDepth, int count)
        {
            for (int i = 0; i < count; i++)
            {
                FolderViewModel folder = new FolderViewModel("Folder: " + currentDepth + " #" + (i+1)) { IsExpanded = true };
                Fill(folder.Children, currentDepth+1, count - 1);
                folders.Add(folder);
            }
        }
	}
}

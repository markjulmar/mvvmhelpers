using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MultiSelectCollectionTest.ViewModels
{
	public class MainViewModel
	{
        public IList<FolderViewModel> RootFolders { get; private set; }
        public IList<FolderViewModel> SelectedFolders { get; private set; }

	    public MainViewModel()
	    {
	        RootFolders = new ObservableCollection<FolderViewModel>();
            SelectedFolders = new ObservableCollection<FolderViewModel>();
            
            for (int i = 0; i < 20; i++)
            {
                RootFolders.Add(new FolderViewModel("Folder #" + (i + 1)));
            }
        }
	}
}

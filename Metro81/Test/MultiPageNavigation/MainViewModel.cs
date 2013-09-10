using System.Collections.ObjectModel;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace App1
{
    public class Module
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [ExportViewModel("Main")]
    public sealed class MainViewModel : ViewModel
    {
        private string _selectedModuleName;
        private Module _selectedModule;
        public IList<Module> Modules { get; private set; }

        /// <summary>
        /// This is used to select a module with no CommandParameter (i.e. using SelectedModule property)
        /// </summary>
        public IDelegateCommand SelectModuleCommand { get; private set; }

        /// <summary>
        /// This is used to select a module from an ElementName.SelectedItem binding
        /// </summary>
        public IDelegateCommand SelectModuleFromElementCommand { get; private set; }

        /// <summary>
        /// This is used to select a module from an ItemClick event, passing EventArgs
        /// </summary>
        public IDelegateCommand SelectModuleFromItemClickCommand { get; private set; }

        public MainViewModel()
        {
            Modules = new ObservableCollection<Module>
            {
                new Module {Name = "Module 1", Description = "A description"},
                new Module {Name = "Module 2", Description = "A description"},
                new Module {Name = "Module 3", Description = "A description"},
                new Module {Name = "Module 4", Description = "A description"}
            };

            SelectModuleCommand = new DelegateCommand(OnSelectModule);
            SelectModuleFromElementCommand = new DelegateCommand<Module>(OnSelectModule);
            SelectModuleFromItemClickCommand = new DelegateCommand<ItemClickEventArgs>(OnSelectModuleFromItemClick);                        
        }

        /// <summary>
        /// Property data bound to selection state of ListView
        /// </summary>
        public Module SelectedModule
        {
            get { return _selectedModule; }
            set { SetPropertyValue(ref _selectedModule, value); }
        }

        /// <summary>
        /// Results printed on screen
        /// </summary>
        public string SelectedModuleName
        {
            get { return _selectedModuleName; }
            set { SetPropertyValue(ref _selectedModuleName,  value); }
        }

        private void OnSelectModuleFromItemClick(ItemClickEventArgs e)
        {
            OnSelectModule(e.ClickedItem as Module);
        }

        private void OnSelectModule()
        {
            OnSelectModule(this.SelectedModule);
        }

        private async void OnSelectModule(Module module)
        {
            if (module != null)
            {
                SelectedModuleName = module.Name;
                await Task.Delay(4000);
                SelectedModuleName = string.Empty;
            }
        }
    }
}

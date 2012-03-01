using System.Windows;
using JulMar.Core.Services;
using System.ComponentModel.Composition.Hosting;

namespace ServiceReplacement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Can add this for a specific folder search for dependencies.
            //MefComposer.Instance.AddCatalogResolver(new DirectoryCatalog("Extensions", "*.dll"));
        }
    }
}

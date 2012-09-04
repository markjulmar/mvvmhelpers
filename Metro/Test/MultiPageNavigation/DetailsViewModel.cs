using System.Collections.ObjectModel;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using JulMar.Core.Services.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace App1
{
    [ExportViewModel("Details")]
    class DetailsViewModel : ViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailsViewModel" /> class.
        /// </summary>
        public DetailsViewModel()
        {
        }
    }
}

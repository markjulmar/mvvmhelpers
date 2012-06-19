using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JulMar.Windows.Mvvm;
using System.Collections.ObjectModel;
using Expression.Blend.SampleData.PODataSource;

namespace MasterDetailTest.ViewModels
{
    public class MainViewModel : SimpleViewModel
    {
        private PurchaseOrderViewModel _selectedOrder;

        public IList<PurchaseOrderViewModel> PurchaseOrders { get; private set; }

        public PurchaseOrderViewModel SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value; 
                RaisePropertyChanged(() => SelectedOrder);
            }
        }

        public MainViewModel()
        {
            PurchaseOrders = new ObservableCollection<PurchaseOrderViewModel>(
                new PODataSource().PurchaseOrders.Select(po => new PurchaseOrderViewModel(po)));
            SelectedOrder = PurchaseOrders.FirstOrDefault();
        }
    }
}

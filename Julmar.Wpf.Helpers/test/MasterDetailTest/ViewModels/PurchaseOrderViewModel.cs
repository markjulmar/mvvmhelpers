using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Expression.Blend.SampleData.PODataSource;
using JulMar.Windows.Mvvm;

namespace MasterDetailTest.ViewModels
{
    public class PurchaseOrderViewModel : SimpleViewModel
    {
        private readonly PurchaseOrdersItem _item;

        public PurchaseOrderViewModel(PurchaseOrdersItem item)
        {
            _item = item;
            OrderDetails = new ObservableCollection<OrderDetailsItemViewModel>(
                _item.OrderDetails.Select(od => new OrderDetailsItemViewModel(od)));
        }

        public IList<OrderDetailsItemViewModel> OrderDetails { get; private set; }

        public string CustomerName
        {
            get { return _item.CustomerName; }
        }

        public string OrderDate
        {
            get { return _item.OrderDate; }
        }

        public string Email
        {
            get { return _item.Email; }
        }
    }
}

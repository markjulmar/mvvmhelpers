using System.Threading;
using Expression.Blend.SampleData.PODataSource;
using JulMar.Windows.Mvvm;

namespace MasterDetailTest.ViewModels
{
    public class OrderDetailsItemViewModel : SimpleViewModel
    {
        private readonly OrderDetailsItem _item;

        public OrderDetailsItemViewModel(OrderDetailsItem item)
        {
            _item = item;
        }

        public int InventoryId
        {
            get { return (int) _item.InventoryId; }
        }

        public string Description
        {
            get
            {
                Thread.Sleep(200);
                return _item.Description;
            }
        }
    }
}
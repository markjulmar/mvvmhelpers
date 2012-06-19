using JulMar.Windows.Mvvm;

namespace ItemsControlDragDropBehavior.TestApp
{
    public class Product : SimpleViewModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetPropertyValue(ref _isSelected, value, () => IsSelected); }
        }

        public override string ToString()
        {
            return string.Format("Product {0} {1:C} {2}", Name, Price, Quantity);
        }
    }
}

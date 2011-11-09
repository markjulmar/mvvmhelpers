using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JulMar.Windows.Mvvm;

namespace ItemsControlDragDropBehavior.TestApp
{
    public class MainViewModel : ViewModel
    {
        public IList<Product> Products { get; private set; }

        public MainViewModel(int count)
        {
            Random rnd = new Random();
            Products = new ObservableCollection<Product>();

            for (int i = 0; i < count; i++)
            {
                Products.Add(new Product
                {
                    Name = "Product #" + (i + 1), 
                    Quantity = rnd.Next(100), 
                    Price = rnd.NextDouble() * 1000
                });
            }
        }

        public void RepositionItems(List<Product> selection, int position)
        {
            foreach (var item in selection.Reverse<Product>())
            {
                int index = Products.IndexOf(item);
                Products.RemoveAt(index);
                if (position > index)
                    position--;
                Products.Insert(position, item);
            }
        }
    }
}
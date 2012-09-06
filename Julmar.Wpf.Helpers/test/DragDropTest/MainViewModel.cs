using System;
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
    }
}
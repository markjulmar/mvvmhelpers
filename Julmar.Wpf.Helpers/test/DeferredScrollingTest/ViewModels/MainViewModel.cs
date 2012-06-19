using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using JulMar.Windows.Mvvm;
using System.Reflection;

namespace DeferredScrollingTest.ViewModels
{
    public class MainViewModel
    {
        public IList<ColorSwatch> Colors { get; private set; }

        public MainViewModel()
        {
            Colors = new ObservableCollection<ColorSwatch>();

            foreach (PropertyInfo property in typeof(Colors).GetProperties())
            {
                ColorSwatch cs = new ColorSwatch();
                cs.Name = property.Name;
                cs.Color = property.GetValue(null, null).ToString();
                cs.HexCode = cs.Color;

                Colors.Add(cs);
            }
        }
    }
}

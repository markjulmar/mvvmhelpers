using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BouncingBalls.Converters
{
    public class RandomColorConverter : IValueConverter
    {
        private Random RNG = new Random();
        private static Brush[] _brushes = null;
        public static Brush[] Brushes
        {
            get
            {
                if (_brushes == null)
                {
                    PropertyInfo[] colors = typeof(Colors).GetTypeInfo().DeclaredProperties.ToArray();
                    _brushes = new Brush[colors.Length];
                    for (int index = 0; index < colors.Length; index++)
                    {
                        Color color = (Color)colors[index].GetValue(null);
                        _brushes[index] = new SolidColorBrush(color);
                    }
                }

                return _brushes;
            }
        }


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var brushes = Brushes;
            return brushes[RNG.Next(brushes.Length)];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

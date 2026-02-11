using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using m = System.Windows.Media;

namespace PocketLLM
{
    public class BubbleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? new m.SolidColorBrush(m.Color.FromRgb(70, 130, 180)) : new m.SolidColorBrush(m.Color.FromRgb(60, 60, 60));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

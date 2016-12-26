using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace APPD.Views.Converters
{
    public class A_X_Plus_B : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
                return 0;

            double a = (double)(values[0] ?? 0.0);
            double x = (double)(values[1] ?? 0.0);
            double b = (double)(values[2] ?? 0.0);

            return a * x + b;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

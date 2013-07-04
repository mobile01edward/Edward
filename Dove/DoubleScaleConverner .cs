using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Dove
{
   public class DoubleScaleConverner : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && targetType == typeof(double))
            {
                double dbOrig = (double)value;
                if (parameter is string)
                {
                    double dbParam;
                    if (double.TryParse((parameter as string), out dbParam))
                    {
                        return Math.Round(dbOrig * dbParam);
                    }
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

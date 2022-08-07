using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YAHAC.Converters
{
    public class NumberToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return "";
                case Double d:
                    return d.ToString("N1", CultureInfo.CreateSpecificCulture("fr-CA"));
                case UInt32 d:
                    return d.ToString("N0", CultureInfo.CreateSpecificCulture("fr-CA"));
                case UInt16 d:
                    return d.ToString("N0", CultureInfo.CreateSpecificCulture("fr-CA"));
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace YAHAC.Converters
{
    public class MemoryStreamToImageSource : IValueConverter
    {
        static uint shitCounter = 0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var input = value as System.IO.Stream;
                input.Seek(0, System.IO.SeekOrigin.Begin);
                BitmapImage image = new();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = input;
                image.EndInit();
                return image;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Why TF you ever wa... just.. dont ok?");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YAHAC.Converters
{
	public class BitmapToMemoryStream : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MemoryStream ms = new MemoryStream();
			(value as Bitmap).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			return ms;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

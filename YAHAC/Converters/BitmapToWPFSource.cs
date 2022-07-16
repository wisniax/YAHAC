using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YAHAC.Converters
{
	/// <summary>
	/// Converts Bitmap to WPF BitmapSource so that images will load :)
	/// </summary>
	public class BitmapToWPFSource : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//return Basic_NoAlpha((Bitmap)value);
			return Complex_PerfectConversion((Bitmap)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		//https://stackoverflow.com/questions/26260654/wpf-converting-bitmap-to-imagesource/26261562#26261562

		/// <summary>
		/// Converts Bitmap to WPF BitmapSource so that images will load :) <br />
		/// Features very lossy and SLOW AF conversion...
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		[Obsolete]
		public BitmapSource Basic_NoAlpha(Bitmap bitmap)
		{
			MemoryStream ms = new MemoryStream();
			((System.Drawing.Bitmap)bitmap).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			ms.Seek(0, SeekOrigin.Begin);
			image.StreamSource = ms;
			image.EndInit();
			return image;
		}

		/// <summary>
		/// Converts Bitmap to WPF BitmapSource so that images will load :) <br />
		/// Features perfect conversion (I hope :))
		/// 25 times faster while using 38 times less memory than Basic conversion  <br />
		/// Thats kinda scam nhl...
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public BitmapSource Complex_PerfectConversion(Bitmap bitmap)
		{
			if (bitmap == null)
				throw new ArgumentNullException("bitmap");

			var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

			var bitmapData = bitmap.LockBits(
				rect,
				ImageLockMode.ReadWrite,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			try
			{
				var size = (rect.Width * rect.Height) * 4;

				return BitmapSource.Create(
					bitmap.Width,
					bitmap.Height,
					bitmap.HorizontalResolution,
					bitmap.VerticalResolution,
					PixelFormats.Bgra32,
					null,
					bitmapData.Scan0,
					size,
					bitmapData.Stride);
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
		}
	}
}

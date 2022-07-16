using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAHAC.Properties;
using YAHAC.Core;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace YAHAC.MVVM.ViewModel
{
	public class ItemViewModel : ObservableObject
	{
		private Bitmap _Cos;

		public Bitmap Cos
		{
			get { return _Cos; }
			set
			{
				_Cos = value;
				OnPropertyChanged();
			}
		}

		public System.Windows.Media.Imaging.BitmapSource Cosie
		{
			//get
			//{
			//	MemoryStream ms = new MemoryStream();
			//	((System.Drawing.Bitmap)Cos).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
			//	BitmapImage image = new BitmapImage();
			//	image.BeginInit();
			//	ms.Seek(0, SeekOrigin.Begin);
			//	image.StreamSource = ms;
			//	image.EndInit();
			//	return image;
			get
			{
				return CreateBitmapSourceFromGdiBitmap(Cos);
			}
			
		}
		//https://stackoverflow.com/questions/26260654/wpf-converting-bitmap-to-imagesource/26261562#26261562
		public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
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

		public ItemViewModel()
		{
			Cos = new Bitmap(Properties.Resources.NoTextureMark);
		}
	}
}

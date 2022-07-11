using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using YAHAC.Core.ApiInstances;

using YAHAC.MVVM.View;

namespace YAHAC.MVVM.ViewModel
{
	internal class BazaarViewModel
	{
		public BazaarView ViewModel { get; set; }
		ObservableCollection<object> items;

		public BazaarViewModel()
		{
			ViewModel = new BazaarView();
			items = new();
			ViewModel.ItemsList.ItemsSource = items;
			//foreach (var item in BazaarCheckup.bazaarObj.products.Keys)
			//{
			//	MinecraftItemBox itemBox = new();
			//	var bitmapImage = new BitmapImage();
			//	bitmapImage.BeginInit();
			//	MemoryStream ms = new MemoryStream();
			//	AllItemsREPO.IDtoITEM(item).Texture.Save(ms, ImageFormat.Bmp);
			//	ms.Seek(0,SeekOrigin.Begin);
			//	bitmapImage.StreamSource = ms;
			//	bitmapImage.EndInit();
			//	itemBox.ImageBox.Source = bitmapImage;
			//	itemBox.UC.Width = Properties.Settings.Default.MinecraftItemBox_Size;
			//	itemBox.UC.Height = Properties.Settings.Default.MinecraftItemBox_Size;
			//	items.Add(itemBox);
			//}
		}
		~BazaarViewModel()
		{
			ViewModel = null;
			items = null;
		}

	}
}

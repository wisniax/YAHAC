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
using YAHAC.Core;
using YAHAC.MVVM.View;

namespace YAHAC.MVVM.ViewModel
{
	internal class BazaarViewModel : ObservableObject
	{
		ObservableCollection<object> items;
		private ObservableCollection<object> _Items;

		public ObservableCollection<object> Items
		{
			get { return _Items; }
			set
			{
				_Items = value;
				OnPropertyChanged();
			}
		}


		public BazaarViewModel()
		{
			items = new();
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

	}
}

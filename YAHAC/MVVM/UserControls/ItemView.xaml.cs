using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ITR;
using YAHAC.MVVM.ViewModel;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for ItemView.xaml
	/// </summary>
	public partial class ItemView : UserControl
	{

		public Item item
		{
			get { return (Item)GetValue(itemProperty); }
			set { if (value != null) SetValue(itemProperty, value); }
		}

		// Using a DependencyProperty as the backing store for item.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty itemProperty =
			DependencyProperty.Register("item", typeof(Item), typeof(ItemView), new PropertyMetadata(
				new Item(null, null, Material.AIR, true, Properties.Resources.NoTextureMark, false)));


		public ItemView(Item item) : this()
		{
			this.item = item;
		}
		public ItemView()
		{
			MainViewModel.itemTextureResolver.DownloadedItemEvent += ItemTextureResolver_DownloadedItemEvent;
			InitializeComponent();
		}

		//https://stackoverflow.com/questions/15504826/invokerequired-in-wpf
		private void ItemTextureResolver_DownloadedItemEvent(ItemTextureResolver source, Item itemUpdated)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() =>
				{
					//string he = itemUpdated.HyPixel_ID;
					//string ha = item.HyPixel_ID;
					if (item == null || itemUpdated == null) return;
					if (itemUpdated.HyPixel_ID != item.HyPixel_ID) return;
					item = new(itemUpdated);
					return;
				});
			}
			else
			{
				if (item == null || itemUpdated == null) return;
				if (itemUpdated.HyPixel_ID != item.HyPixel_ID) return;
				item = new(itemUpdated);
			}
		}
	}
}

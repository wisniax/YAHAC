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
using SharpNBT;
using YAHAC.Core;
using YAHAC.MVVM.ViewModel;
using YAHAC.Properties;

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

		private static System.IO.MemoryStream ImageToStream(System.Drawing.Bitmap bitmap)
		{
			var stream = new System.IO.MemoryStream();
			bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
			return stream;
		}

		// Using a DependencyProperty as the backing store for item.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty itemProperty =
			DependencyProperty.Register("item", typeof(Item), typeof(ItemView), new PropertyMetadata(
				new Item(null, null, Material.AIR, true, ImageToStream(Properties.Resources.NoTextureMark), false)));


		public int ItemBoxSize
		{
			get { return (int)GetValue(ItemBoxSizeProperty); }
			set { SetValue(ItemBoxSizeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ItemBoxSize.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ItemBoxSizeProperty =
			DependencyProperty.Register("ItemBoxSize", typeof(int), typeof(ItemView), new PropertyMetadata(34));



		public bool visible
		{
			get { return (bool)GetValue(visibleProperty); }
			set { SetValue(visibleProperty, value); }
		}

		// Using a DependencyProperty as the backing store for visible.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty visibleProperty =
			DependencyProperty.Register("visible", typeof(bool), typeof(ItemView), new PropertyMetadata(false));



		public ItemToSearchFor itemToSearchFor
		{
			get { return (ItemToSearchFor)GetValue(itemToSearchForProperty); }
			set { SetValue(itemToSearchForProperty, value); }
		}

		// Using a DependencyProperty as the backing store for itemToSearchFor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty itemToSearchForProperty =
			DependencyProperty.Register("itemToSearchFor", typeof(ItemToSearchFor), typeof(ItemView), new FrameworkPropertyMetadata(
			null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


		public ItemView(Item item, object Tag = null, bool visible = false, ItemToSearchFor itemToSearchFor = null) : this()
		{
			this.item = item;
			this.Tag = Tag;
			this.visible = visible;
			this.itemToSearchFor = itemToSearchFor;
		}
		public ItemView()
		{
			ItemBoxSize = MainViewModel.settings.Default.MinecraftItemBox_Size;
			MainViewModel.itemTextureResolver.DownloadedItemEvent += ItemTextureResolver_DownloadedItemEvent;
			InitializeComponent();
		}

		//https://stackoverflow.com/questions/15504826/invokerequired-in-wpf
		private void ItemTextureResolver_DownloadedItemEvent(ItemTextureResolver source, Item itemUpdated)
		{
			if (!Dispatcher.CheckAccess())
			{
				try
				{
					Dispatcher.Invoke(() =>
					{
						if (item == null || itemUpdated == null) return;
						if (itemUpdated.HyPixel_ID != item.HyPixel_ID) return;
						item = new(itemUpdated);
						return;
					});
				}
				catch (TaskCanceledException e)
				{
					return;
				}
			}
			else
			{
				if (item == null || itemUpdated == null) return;
				if (itemUpdated.HyPixel_ID != item.HyPixel_ID) return;
				item = new(itemUpdated);
			}
		}

		private void Delete_Btn_Click(object sender, RoutedEventArgs e)
		{
			if (itemToSearchFor == null) return;
			MainViewModel.betterAH.RemoveRecipe(itemToSearchFor.recipe_key);
		}

		private void Modify_Btn_Click(object sender, RoutedEventArgs e)
		{
			if (itemToSearchFor == null) return;

		}

		private void Dupe_Btn_Click(object sender, RoutedEventArgs e)
		{
			if (itemToSearchFor == null) return;
			MainViewModel.betterAH.AddRecipe(itemToSearchFor);
		}
	}
}

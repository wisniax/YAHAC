﻿using System;
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
using ABI.Windows.ApplicationModel.Chat;
using ITR;
using SharpNBT;
using YAHAC.Core;
using YAHAC.MVVM.Model;
using YAHAC.MVVM.View;
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
			DependencyProperty.Register("item", typeof(Item), typeof(ItemView));


		public int ItemBoxSize
		{
			get { return (int)GetValue(ItemBoxSizeProperty); }
			set { SetValue(ItemBoxSizeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ItemBoxSize.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ItemBoxSizeProperty =
			DependencyProperty.Register("ItemBoxSize", typeof(int), typeof(ItemView));



		public bool visible
		{
			get { return (bool)GetValue(visibleProperty); }
			set { SetValue(visibleProperty, value); }
		}

		// Using a DependencyProperty as the backing store for visible.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty visibleProperty =
			DependencyProperty.Register("visible", typeof(bool), typeof(ItemView));



		public ItemToSearchFor itemToSearchFor
		{
			get { return (ItemToSearchFor)GetValue(itemToSearchForProperty); }
			set { SetValue(itemToSearchForProperty, value); }
		}

		// Using a DependencyProperty as the backing store for itemToSearchFor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty itemToSearchForProperty =
			DependencyProperty.Register("itemToSearchFor", typeof(ItemToSearchFor), typeof(ItemView));

		public delegate void ItemModifyRequestedHandler(ItemView source);
		public event ItemModifyRequestedHandler ItemModifyRequestedEvent;

		public void Reuse(Item item, object Tag = null, bool visible = false, ItemToSearchFor itemToSearchFor = null)
		{
			this.item = item;
			this.Tag = Tag;
			this.visible = visible;
			this.itemToSearchFor = itemToSearchFor;
		}
		public ItemView(Item item, object Tag = null, bool visible = false, ItemToSearchFor itemToSearchFor = null)
		{
			ItemBoxSize = MainViewModel.Settings?.Default?.MinecraftItemBox_Size ?? 34;
			MainViewModel.itemTextureResolver.DownloadedItemEvent += ItemTextureResolver_DownloadedItemEvent;
			this.item = item;
			this.Tag = Tag;
			this.visible = visible;
			this.itemToSearchFor = itemToSearchFor;
			InitializeComponent();
		}
		public ItemView()
		{
			visible = false;
			this.item = new Item(null, null, Material.AIR, true, MainViewModel.NoTextureMarkItem, false);
			ItemBoxSize = MainViewModel.Settings?.Default?.MinecraftItemBox_Size ?? 34;
			MainViewModel.itemTextureResolver.DownloadedItemEvent += ItemTextureResolver_DownloadedItemEvent;
			InitializeComponent();
		}

		public void PrepareToDie()
		{
			MainViewModel.itemTextureResolver.DownloadedItemEvent -= ItemTextureResolver_DownloadedItemEvent;
			this.DataContext = null;
		}

		private void ItemModifyRequested()
		{
			ItemModifyRequestedEvent?.Invoke(this);
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
				catch (TaskCanceledException)
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
			if (!visible) return;
			if (itemToSearchFor != null) MainViewModel.betterAH.RemoveRecipe(itemToSearchFor.recipe_key);
			if (Tag is ItemsToSearchForCatalogue) MainViewModel.betterAH.RemoveCatalogue(Tag as ItemsToSearchForCatalogue);
		}

		private void Modify_Btn_Click(object sender, RoutedEventArgs e)
		{
			if (!visible) return;
			if (itemToSearchFor != null || Tag != null) ItemModifyRequested();
		}

		private void Dupe_Btn_Click(object sender, RoutedEventArgs e)
		{
			if (itemToSearchFor != null) MainViewModel.betterAH.AddRecipe(itemToSearchFor);
		}

		private void MoveLeft_Btn_Click(object sender, RoutedEventArgs e)
		{
			if (itemToSearchFor != null) MainViewModel.betterAH.MoveRecipe(itemToSearchFor, FlowDirection.RightToLeft);
			if (Tag is ItemsToSearchForCatalogue) MainViewModel.betterAH.MoveCatalogue(Tag as ItemsToSearchForCatalogue, FlowDirection.RightToLeft);
		}

		private void MoveRight_Btn_Click(object sender, RoutedEventArgs e)
		{
			if (itemToSearchFor != null) MainViewModel.betterAH.MoveRecipe(itemToSearchFor, FlowDirection.LeftToRight);
			if (Tag is ItemsToSearchForCatalogue) MainViewModel.betterAH.MoveCatalogue(Tag as ItemsToSearchForCatalogue, FlowDirection.LeftToRight);
		}
	}
}

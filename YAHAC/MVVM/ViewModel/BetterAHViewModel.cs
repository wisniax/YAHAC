using ITR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YAHAC.Core;
using YAHAC.MVVM.Model;
using YAHAC.MVVM.UserControls;
using YAHAC.MVVM.View;
using YAHAC.Properties;

namespace YAHAC.MVVM.ViewModel
{
	internal class BetterAHViewModel : ObservableObject
	{
		public RelayCommand BetterAHSettings { get; private set; }
		public RelayCommand AddItemInComboBox { get; private set; }

		private ObservableCollection<object> _Items;
		/// <summary>
		/// Collection of auctions in matchingItems list presented in graphical format
		/// </summary>
		public ObservableCollection<object> Items
		{
			get { return _Items; }
			set
			{
				_Items = value;
				OnPropertyChanged();
			}
		}
		private bool _AdditonalInfo_Visible;
		public bool AdditionalInfo_Visible
		{
			get { return _AdditonalInfo_Visible; }
			set
			{
				_AdditonalInfo_Visible = value;
				OnPropertyChanged();
			}
		}
		private ItemView _SelectedItemView;
		public ItemView SelectedItemView
		{
			get { return _SelectedItemView; }
			set
			{
				if (value == null) { AdditionalInfo_Visible = false; return; }
				_SelectedItemView = value;
				OnPropertyChanged();
				AdditionalInfo_Visible = true;
			}
		}
		private Point _CanvasPoint;
		public Point CanvasPoint
		{
			get { return _CanvasPoint; }
			set
			{
				_CanvasPoint = value;
				OnPropertyChanged();
			}
		}
		private ObservableCollection<ItemView> _ItemsToSearchForCollection;
		public ObservableCollection<ItemView> ItemsToSearchForCollection
		{
			get { return _ItemsToSearchForCollection; }
			set
			{
				_ItemsToSearchForCollection = value;
				OnPropertyChanged();
			}
		}
		private bool _ItemsToSearchForVisibility;
		public bool ItemsToSearchForVisibility
		{
			get { return _ItemsToSearchForVisibility; }
			set
			{
				_ItemsToSearchForVisibility = value;
				LoadItemsToSearchForCollection();
				OnPropertyChanged();
			}
		}
		private ItemToSearchFor _SelectedItemToRecipeConfig;
		public ItemToSearchFor SelectedItemToRecipeConfig
		{
			get { return _SelectedItemToRecipeConfig; }
			set
			{
				_SelectedItemToRecipeConfig = value;
				OnPropertyChanged();
			}
		}

		public BetterAHViewModel()
		{
			BetterAHSettings = new RelayCommand((o) => { ItemsToSearchForVisibility = !ItemsToSearchForVisibility; });
			AddItemInComboBox = new RelayCommand((o) =>
			{
				MainViewModel.betterAH.AddRecipe(new Properties.ItemToSearchFor(null, enabled: false));
			});
			ItemsToSearchForVisibility = false;
			Items = new();
			ItemsToSearchForCollection = new();
			MainViewModel.betterAH.BetterAHUpdatedEvent += BetterAH_Updated;
			if (Items.Count == 0 && MainViewModel.betterAH.success) LoadBetterAH();
		}

		void LoadBetterAH()
		{
			Items = new();
			if (!MainViewModel.betterAH.success) return;
			foreach (var auction in MainViewModel.betterAH.MatchingItems)
			{
				if (auction == null) continue;

				var item = MainViewModel.itemTextureResolver.GetItemFromID(auction.HyPixel_ID);
				//if (item == null) continue;
				// In case item does not exist in Hypixel API create an unknown one with id as its name
				if (item == null)
				{
					Converters.BitmapToMemoryStream convbtm = new Converters.BitmapToMemoryStream();
					item = new ITR.Item(
						  auction.HyPixel_ID,
						  auction.HyPixel_ID,
						  Material.AIR,
						  true,
						  convbtm.Convert(Properties.Resources.NoTextureMark, null, null, CultureInfo.CurrentCulture) as MemoryStream);
				}
				ItemView itemBox = new(item, auction);
				Items?.Add(itemBox);
			}
		}

		void LoadItemsToSearchForCollection()
		{
			if (!ItemsToSearchForVisibility) return;
			if (ItemsToSearchForCollection == null) return;

			//foreach (var item in ItemsToSearchForCollection)
			//{
			//	item.PrepareToDie();
			//}

			//ItemsToSearchForCollection.Clear();
			for (int i = 0; i < MainViewModel.betterAH.ItemsToSearchFor.Count; i++)
			{
				var itemToSearchFor = MainViewModel.betterAH.ItemsToSearchFor[i];
				if (itemToSearchFor == null) continue;
				var item = ItemsToSearchForCollection.FirstOrDefault((a) => a.itemToSearchFor == itemToSearchFor, null);
				if (ItemsToSearchForCollection.IndexOf(item) != i)
				{
					if (ItemsToSearchForCollection.IndexOf(item) == -1)
					{
						var mcItem = MainViewModel.itemTextureResolver.GetItemFromID(itemToSearchFor.item_dictKey);
						if (mcItem == null)
						{
							Converters.BitmapToMemoryStream convbtm = new Converters.BitmapToMemoryStream();
							mcItem = new Item(
								  itemToSearchFor.item_dictKey,
								  itemToSearchFor.item_dictKey,
								  Material.AIR,
								  true,
								  convbtm.Convert(Properties.Resources.NoTextureMark, null, null, CultureInfo.CurrentCulture) as MemoryStream);
						}
						var auction = MainViewModel.betterAH.FindCheapestMatchingItem(itemToSearchFor);
						ItemView itemBox = new(mcItem, auction, true, itemToSearchFor);
						itemBox.ItemModifyRequestedEvent += ItemToSearchForModifyRequested;
						ItemsToSearchForCollection?.Insert(i, itemBox);
					}
					else
					{
						ItemsToSearchForCollection.Move(ItemsToSearchForCollection.IndexOf(item), i);
					}

				}
				else
				{
					if (item.item.HyPixel_ID != itemToSearchFor.item_dictKey)
					{
						var mcItem = MainViewModel.itemTextureResolver.GetItemFromID(itemToSearchFor.item_dictKey);
						if (mcItem == null)
						{
							Converters.BitmapToMemoryStream convbtm = new Converters.BitmapToMemoryStream();
							mcItem = new Item(
								  itemToSearchFor.item_dictKey,
								  itemToSearchFor.item_dictKey,
								  Material.AIR,
								  true,
								  convbtm.Convert(Properties.Resources.NoTextureMark, null, null, CultureInfo.CurrentCulture) as MemoryStream);
						}
						item.item = mcItem;
					}
					var auction = MainViewModel.betterAH.FindCheapestMatchingItem(itemToSearchFor);
					item.Tag = auction;
				}
			}
			for (int i = ItemsToSearchForCollection.Count - 1; i >= MainViewModel.betterAH.ItemsToSearchFor.Count; i--)
			{
				ItemsToSearchForCollection[i].PrepareToDie();
				ItemsToSearchForCollection.RemoveAt(i);
			}
			//foreach (var item in ItemsToSearchForCollection.ToArray())
			//{
			//	if (MainViewModel.betterAH.ItemsToSearchFor.Contains(item.itemToSearchFor)) continue;
			//	item.PrepareToDie();
			//	ItemsToSearchForCollection.Remove(item);
			//}
		}

		private void ItemToSearchForModifyRequested(ItemView source)
		{
			if (source == null) return;
			if (source.itemToSearchFor == null) return;
			SelectedItemToRecipeConfig = null;
			SelectedItemToRecipeConfig = source.itemToSearchFor;
		}

		private void BetterAH_Updated(Model.BetterAH source)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (source == null) return;
					if (!source.success) return;
					LoadBetterAH();
					LoadItemsToSearchForCollection();
					return;
				});
			}
			else
			{
				if (source == null) return;
				if (!source.success) return;
				LoadBetterAH();
				LoadItemsToSearchForCollection();
				return;
			}
		}

		//private void RefreshItemsToSearchForCollection()
		//{
		//	if (!ItemsToSearchForVisibility) return;
		//	if (ItemsToSearchForCollection == null) return;
		//	foreach (var ite in ItemsToSearchForCollection)
		//	{
		//		if (ite == null) continue;

		//		var auction = MainViewModel.betterAH.FindCheapestMatchingItem(ite.itemToSearchFor);
		//		ite.Tag = auction;
		//	}
		//}

		public void MouseDoubleClicked(object sender, MouseButtonEventArgs e)
		{
			if (SelectedItemView == null) return;
			if (SelectedItemView.Tag == null) return;
			CopyToClipboard.Copy("/viewauction " + (SelectedItemView.Tag as Auction).uuid);
		}
	}
}

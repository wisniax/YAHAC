using ITR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
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
		private ObservableCollection<object> _ItemsToSearchForCollection;
		public ObservableCollection<object> ItemsToSearchForCollection
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
		private ObservableCollection<object> _AuctionableItems;
		public ObservableCollection<object> AuctionableItems
		{
			get { return _AuctionableItems; }
			set
			{
				_AuctionableItems = value;
				OnPropertyChanged();
			}
		}
		private object _SelectedAuctionableItem;
		public object SelectedAuctionableItem
		{
			get { return _SelectedAuctionableItem; }
			set
			{
				_SelectedAuctionableItem = value;
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
			SelectedAuctionableItem = new();
			BetterAHSettings = new RelayCommand((o) => { ItemsToSearchForVisibility = !ItemsToSearchForVisibility; });
			AddItemInComboBox = new RelayCommand((o) =>
			{
				if (SelectedAuctionableItem as string == null) return;
				MainViewModel.betterAH.AddRecipe(new Properties.ItemToSearchFor(SelectedAuctionableItem as string, enabled: false));
			});
			ItemsToSearchForVisibility = false;
			Items = new();
			AuctionableItems = new();
			ItemsToSearchForCollection = new();
			MainViewModel.betterAH.BetterAHUpdatedEvent += BetterAH_Updated;
			if (Items.Count == 0 && MainViewModel.betterAH.success) LoadBetterAH();
			//if (MainViewModel.settings.Default.PlaySound)
			//{
			//	soundPlayer = new(Properties.Resources.notify_sound);
			//}
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
					item = new Item(
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
			if (!ItemsToSearchForVisibility)
			{
				if (ItemsToSearchForCollection == null)
                return;

				foreach (ItemView item in ItemsToSearchForCollection)
				{
					item.PrepareToDie();
                }
				ItemsToSearchForCollection = null;
                return;
			}
			ItemsToSearchForCollection = new();
			AuctionableItems = new(MainViewModel.auctionHouse.auctions.Keys);
            foreach (var itemToSearchFor in MainViewModel.betterAH.ItemsToSearchFor)
			{
				if (itemToSearchFor == null) return;

				var item = MainViewModel.itemTextureResolver.GetItemFromID(itemToSearchFor.item_dictKey);
				if (item == null)
				{
					Converters.BitmapToMemoryStream convbtm = new Converters.BitmapToMemoryStream();
					item = new Item(
						  itemToSearchFor.item_dictKey,
						  itemToSearchFor.item_dictKey,
						  Material.AIR,
						  true,
						  convbtm.Convert(Properties.Resources.NoTextureMark, null, null, CultureInfo.CurrentCulture) as MemoryStream);
				}

				var auction = MainViewModel.betterAH.FindCheapestMatchingItem(itemToSearchFor);
				ItemView itemBox = new(item, auction, true, itemToSearchFor);
				itemBox.ItemModifyRequestedEvent += ItemToSearchForModifyRequested;
				 ItemsToSearchForCollection?.Add(itemBox);
			}
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

		public void MouseDoubleClicked(object sender, MouseButtonEventArgs e)
		{
			if (SelectedItemView == null) return;
			if (SelectedItemView.Tag == null) return;
			CopyToClipboard.Copy("/viewauction " + (SelectedItemView.Tag as Auction).uuid);
		}
	}
}

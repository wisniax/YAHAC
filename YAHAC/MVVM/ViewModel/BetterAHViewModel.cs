using System;
using ITR;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using YAHAC.Core;
using YAHAC.MVVM.Model;
using YAHAC.MVVM.UserControls;
using YAHAC.Properties;

namespace YAHAC.MVVM.ViewModel
{
	internal class BetterAhViewModel : ObservableObject
	{
		public RelayCommand BetterAhSettings { get; private set; }
		public RelayCommand AddItemInComboBox { get; private set; }
		public RelayCommand AddCatalogue { get; private set; }

		private ObservableCollection<object> _items;
		/// <summary>
		/// Collection of auctions in matchingItems list presented in graphical format
		/// </summary>
		public ObservableCollection<object> Items
		{
			get => _items;
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}
		private bool _additonalInfoVisible;
		public bool AdditionalInfoVisible
		{
			get => _additonalInfoVisible;
			set
			{
				_additonalInfoVisible = value;
				OnPropertyChanged();
			}
		}
		private ItemView _selectedItemView;
		public ItemView SelectedItemView
		{
			get => _selectedItemView;
			set
			{
				if (value == null) { AdditionalInfoVisible = false; return; }
				_selectedItemView = value;
				OnPropertyChanged();
				AdditionalInfoVisible = true;
			}
		}
		private ItemsToSearchForCatalogue _selectedCatalogue;
		public ItemsToSearchForCatalogue SelectedCatalogue
		{
			get => _selectedCatalogue;
			set
			{
				if (isLoadItemsToSearchForInCatalogueExecuting) return;
				_selectedCatalogue = value;
				LoadItemsToSearchForInCatalogue();
				OnPropertyChanged();
			}
		}
		private Point _canvasPoint;
		public Point CanvasPoint
		{
			get => _canvasPoint;
			set
			{
				_canvasPoint = value;
				OnPropertyChanged();
			}
		}
		private ObservableCollection<ItemView> _itemsToSearchForCollection;
		public ObservableCollection<ItemView> ItemsToSearchForCollection
		{
			get => _itemsToSearchForCollection;
			set
			{
				_itemsToSearchForCollection = value;
				OnPropertyChanged();
			}
		}
		private ObservableCollection<ItemView> _catalogues;
		public ObservableCollection<ItemView> Catalogues
		{
			get => _catalogues;
			set
			{
				_catalogues = value;
				OnPropertyChanged();
			}
		}
		private bool _itemsToSearchForVisibility;
		public bool ItemsToSearchForVisibility
		{
			get => _itemsToSearchForVisibility;
			set
			{
				_itemsToSearchForVisibility = value;
				LoadItemsToSearchForInCatalogue();
				OnPropertyChanged();
			}
		}
		private object _selectedItemToRecipeConfig;
		public object SelectedItemToRecipeConfig
		{
			get => _selectedItemToRecipeConfig;
			set
			{
				_selectedItemToRecipeConfig = value;
				OnPropertyChanged();
			}
		}

		public BetterAhViewModel()
		{
			BetterAhSettings = new RelayCommand((_) => { ItemsToSearchForVisibility = !ItemsToSearchForVisibility; });
			AddItemInComboBox = new RelayCommand((_) =>
			{
				ItemToSearchFor itemek = null;
				if (Keyboard.IsKeyDown(Key.LeftShift)) itemek = ReadFromCowlectionNbt();
				itemek ??= new Properties.ItemToSearchFor(null, enabled: false);
				MainViewModel.betterAH.AddRecipe(itemek, catalogue: SelectedCatalogue);
			});
			AddCatalogue = new RelayCommand((_) =>
			{
				MainViewModel.betterAH.AddNewCatalogue("New Catalogue");
			});
			ItemsToSearchForVisibility = false;
			Items = new();
			ItemsToSearchForCollection = new();
			Catalogues = new();
			MainViewModel.betterAH.BetterAHUpdatedEvent += BetterAH_Updated;
			if (Items.Count == 0 && MainViewModel.betterAH.success) LoadBetterAh();
		}

		private ItemToSearchFor ReadFromCowlectionNbt()
		{
			var itemTag = NBTReader.ReadCowlectionNbtFromClipboard();
			if (itemTag?.tag.ExtraAttributes == null) return null;
			if (itemTag.tag.ExtraAttributes.attributes == null) return new ItemToSearchFor(itemTag.tag.ExtraAttributes.id);
			var str = itemTag.tag.ExtraAttributes.attributes.ToString()!.Trim();
			var regex = new Regex("(?<=\").+?(?=\")");
			var matches = regex.Matches(str).ToList();
			List<string> queries = new();
			foreach (var query in matches)
			{
				var tmp = query.Value.Replace('_', ' ');
				tmp = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tmp.ToLower());
				queries.Add(tmp);
			}
			return new ItemToSearchFor(itemTag.tag.ExtraAttributes.id, searchQueries: queries, enabled: false);
		}

		void LoadBetterAh()
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
					item = new ITR.Item(
						  auction.HyPixel_ID,
						  auction.HyPixel_ID,
						  Material.AIR,
						  true,
						  MainViewModel.NoTextureMarkItem);
				}
				ItemView itemBox = new(item, auction);
				Items?.Add(itemBox);
			}
		}

		void LoadCatalogues()
		{
			Catalogues = new();
			if (!MainViewModel.betterAH.success) return;
			foreach (var catalogue in MainViewModel.betterAH.ItemsToSearchForCatalogues)
			{
				if (catalogue == null) continue;
				var mcItem = new Item(
					catalogue.Name,
					catalogue.ID,
					Material.CHEST,
					false,
					MainViewModel.itemTextureResolver.GetItemFromID(Material.CHEST.ToString())?.Texture);
				ItemView itemBox = new(mcItem, catalogue, true);
				itemBox.ItemModifyRequestedEvent += ItemToSearchForModifyRequested;
				Catalogues?.Add(itemBox);
			}
		}

		private bool isLoadItemsToSearchForInCatalogueExecuting = false;
		async void LoadItemsToSearchForInCatalogue()
		{
			if (isLoadItemsToSearchForInCatalogueExecuting) return;
			if (!ItemsToSearchForVisibility) return;
			if (ItemsToSearchForCollection == null) return;
			if (SelectedCatalogue != null && !MainViewModel.betterAH.ItemsToSearchForCatalogues.Contains(SelectedCatalogue))
			{
				SelectedCatalogue = null;
				return;
			}

			isLoadItemsToSearchForInCatalogueExecuting = true;
			List<Task> tasks = new();
			for (int i = 0; i < SelectedCatalogue?.Items.Count; i++)
			{
				var itemToSearchFor = SelectedCatalogue.Items[i];
				if (itemToSearchFor == null) continue;
				var item = ItemsToSearchForCollection.FirstOrDefault((a) => a.itemToSearchFor == itemToSearchFor, null);

				if ((ItemsToSearchForCollection.IndexOf(item) == i))
				{
					if (item.item.HyPixel_ID != itemToSearchFor.item_dictKey)
					{
						var mcItem = MainViewModel.itemTextureResolver.GetItemFromID(itemToSearchFor.item_dictKey) ??
									 new Item(
							itemToSearchFor.item_dictKey,
							itemToSearchFor.item_dictKey,
							Material.AIR,
							true,
							MainViewModel.NoTextureMarkItem);
						item.item = mcItem;
					}
					var auction = MainViewModel.betterAH.FindCheapestMatchingItem(itemToSearchFor);
					item.Tag = auction;
				}
				else if (ItemsToSearchForCollection.IndexOf(item) == -1 && ItemsToSearchForCollection.Count - 1 > i)
				{
					var mcItem = MainViewModel.itemTextureResolver.GetItemFromID(itemToSearchFor.item_dictKey);
					mcItem ??= new Item(
						itemToSearchFor.item_dictKey,
						itemToSearchFor.item_dictKey,
						Material.AIR,
						true,
						MainViewModel.NoTextureMarkItem);
					var auction = MainViewModel.betterAH.FindCheapestMatchingItem(itemToSearchFor);
					ItemsToSearchForCollection[i].Reuse(mcItem, auction, true, itemToSearchFor);
				}
				else if (ItemsToSearchForCollection.IndexOf(item) == -1)
				{
					var mcItem = MainViewModel.itemTextureResolver.GetItemFromID(itemToSearchFor.item_dictKey);
					mcItem ??= new Item(
						itemToSearchFor.item_dictKey,
						itemToSearchFor.item_dictKey,
						Material.AIR,
						true,
						MainViewModel.NoTextureMarkItem);
					var auction = MainViewModel.betterAH.FindCheapestMatchingItem(itemToSearchFor);
					int j = i;
					tasks.Add(Application.Current.Dispatcher.BeginInvoke(
						DispatcherPriority.Background,
						() =>
						{
							ItemView itemBox = new(mcItem, auction, true, itemToSearchFor);
							itemBox.ItemModifyRequestedEvent += ItemToSearchForModifyRequested;
							ItemsToSearchForCollection?.Insert(j, itemBox);
						}).Task);
				}
				else
				{
					var auction = MainViewModel.betterAH.FindCheapestMatchingItem(itemToSearchFor);
					item.Tag = auction;
					ItemsToSearchForCollection.Move(ItemsToSearchForCollection.IndexOf(item), i);
				}
			}
			await Task.WhenAll(tasks);
			for (int i = ItemsToSearchForCollection.Count - 1; i >= (SelectedCatalogue?.Items.Count ?? 0); i--)
			{
				ItemsToSearchForCollection[i].PrepareToDie();
				ItemsToSearchForCollection.RemoveAt(i);
			}
			isLoadItemsToSearchForInCatalogueExecuting = false;
			//foreach (var item in ItemsToSearchForCollection.ToArray())
			//{
			//	if (MainViewModel.betterAH.ItemsToSearchFor.Contains(item.itemToSearchFor)) continue;
			//	item.PrepareToDie();
			//	ItemsToSearchForCollection.Remove(item);
			//}
		}
		[Obsolete("Use UpdateItemsToSearchForCollection instead", true)]
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
						mcItem ??= new Item(
							itemToSearchFor.item_dictKey,
							itemToSearchFor.item_dictKey,
							Material.AIR,
							true,
							MainViewModel.NoTextureMarkItem);
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
							mcItem = new Item(
								  itemToSearchFor.item_dictKey,
								  itemToSearchFor.item_dictKey,
								  Material.AIR,
								  true,
								  MainViewModel.NoTextureMarkItem);
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
			if (source?.itemToSearchFor != null)
			{
				SelectedItemToRecipeConfig = null;
				SelectedItemToRecipeConfig = source.itemToSearchFor;
			}

			if (source?.Tag is ItemsToSearchForCatalogue)
			{
				SelectedItemToRecipeConfig = null;
				SelectedItemToRecipeConfig = source.Tag;
			}
		}

		private void BetterAH_Updated(Model.BetterAH source)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (source is not { success: true }) return;
					LoadBetterAh();
					LoadCatalogues();
					LoadItemsToSearchForInCatalogue();
					return;
				});
			}
			else
			{
				if (source is not { success: true }) return;
				LoadBetterAh();
				LoadCatalogues();
				LoadItemsToSearchForInCatalogue();
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
			switch (SelectedItemView?.Tag)
			{
				case Auction auction:
					CopyToClipboard.Copy("/viewauction " + auction.uuid);
					break;
				case ItemsToSearchForCatalogue itemToSearchForCat:
					SelectedCatalogue = itemToSearchForCat;
					break;
			}
		}
	}
}

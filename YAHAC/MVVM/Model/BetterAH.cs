﻿using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using YAHAC.Core;
using YAHAC.MVVM.ViewModel;
using YAHAC.Properties;

namespace YAHAC.MVVM.Model
{
	internal class BetterAH
	{
		/// <summary>
		/// locker used to lock variable for async use
		/// </summary>
		private readonly object locker = new object();
		/// <summary>
		/// Items to search for in AH from settings
		/// </summary>
		//public List<ItemToSearchFor> ItemsToSearchFor { get; private set; }
		public List<ItemsToSearchForCatalogue> ItemsToSearchForCatalogues { get; private set; }
		/// <summary>
		/// List of auctions that match the query
		/// </summary>
		public List<Auction> MatchingItems { get; private set; }
		/// <summary>
		/// Auction that got highlighted last time
		/// </summary>
		string highlitedAuction_uuid;
		long lastCalculated;
		public bool success { get; private set; }
		public List<ItemToSearchFor> ItemsToSearchFor => ItemsToSearchForCatalogues.SelectMany(x => x.Items).ToList();
		SoundPlayer soundPlayer;
		public delegate void BetterAHUpdatedHandler(BetterAH source);
		public event BetterAHUpdatedHandler BetterAHUpdatedEvent;

		public BetterAH()
		{
			success = false;
			MatchingItems = new();
			soundPlayer = new(Properties.Resources.notify_sound);
			highlitedAuction_uuid = new("");
			LoadRecipes();
			ItemsToSearchForCatalogues ??= new();
			MainViewModel.auctionHouse.AHUpdatedEvent += AuctionHouse_Updated;
		}

		private async void AuctionHouse_Updated(AuctionHouse source)
		{
			if (source is not { success: true }) return;
			await FindAllMatchingItemsAsync();
			FindHighestPriorityAuction();
		}

		/// <summary>
		/// Executed everytime refresh succeded
		/// </summary>
		private void OnBetterAHUpdated()
		{
			BetterAHUpdatedEvent?.Invoke(this);
		}

		private async Task FindAllMatchingItemsAsync()
		{
			List<Auction> tempmatchingItems = new();
			List<Task<List<Auction>>> tasks = new();
			foreach (var catalogue in ItemsToSearchForCatalogues)
			{
				var catFinal = catalogue;
				tasks.Add(Task.Run(() => FindMatchingItemsInCatalogue(catFinal.Items)));
			}
			await Task.WhenAll(tasks);
			tempmatchingItems.AddRange(tasks.SelectMany(x => x.Result));
			//MatchingItems = new();
			MatchingItems = tempmatchingItems;
			lastCalculated = MainViewModel.auctionHouse.lastUpdated;
			success = true;
			OnBetterAHUpdated();
		}

		private void FindAllMatchingItems()
		{
			List<Auction> tempmatchingItems = new();
			foreach (var catalogue in ItemsToSearchForCatalogues)
			{
				tempmatchingItems.AddRange(FindMatchingItemsInCatalogue(catalogue.Items));
			}
			//MatchingItems = new();
			MatchingItems = tempmatchingItems;
			lastCalculated = MainViewModel.auctionHouse.lastUpdated;
			success = true;
			OnBetterAHUpdated();
		}

		private List<Auction> FindMatchingItemsInCatalogue(List<ItemToSearchFor> ItemsToSearchFor)
		{
			List<Auction> tempmatchingItems = new();
			if (ItemsToSearchFor.Count == 0 || MainViewModel.auctionHouse is not { success: true }) { success = false; return new List<Auction>(); }
			foreach (var item in ItemsToSearchFor.Where((a) => a.enabled))
			{
				checkIfItemsMatch(item, tempmatchingItems);
			}
			tempmatchingItems.Sort((a, b) => a.starting_bid.CompareTo(b.starting_bid));
			return tempmatchingItems;
		}

		private void DeleteDuplicatesInMatchingItems(List<Auction> auctionsToSort)
		{
			auctionsToSort = auctionsToSort.GroupBy(x => x.uuid).Select(x => x.First()).ToList();
		}

		/// <summary>
		/// Searches AH for specific item query
		/// </summary>
		/// <param name="item">Query to look AH on</param>
		/// <param name="tempmatchingItems">Adds all items that match the query to this list</param>
		void checkIfItemsMatch(ItemToSearchFor item, List<Auction> tempmatchingItems)
		{
			{
				//Get list of items on AH that match ID
				//if (!MainViewModel.auctionHouse.auctions.ContainsKey(item.item_dictKey)) { return; }
				//var itemsToSearchOn = MainViewModel.auctionHouse.auctions[item.item_dictKey];
				if (!MainViewModel.auctionHouse.auctions.TryGetValue(item.item_dictKey, out var itemsToSearchOn)) { return; }

				//Get the ones that match price and query
				foreach (var entry in itemsToSearchOn)
				{
					if (!entry.bin) continue;                           //Skip if not bin
					if (entry.starting_bid > item.maxPrice) continue;   //Skip if price's too high
					bool doesMatch = true;
					//Skip if lore doesn't contain text

					foreach (var text in item.searchQueries)
					{
						if (!entry.item_lore.Contains(text)) { doesMatch = false; break; }
					}
					if (!doesMatch) continue;

					//Finally add matching item to list
					lock (locker)
					{
						tempmatchingItems.Add(entry);
					}
				}
				//if (matchingItems.Count == 0) continue;					//test whether any items were added
			}
		}

		/// <summary>
		/// Finds Highest Priority auction. <br/>
		/// If playSound enabled even notifies about its finding
		/// </summary>
		/// <returns></returns>
		public Auction FindHighestPriorityAuction()
		{
			var lista = ItemsToSearchFor;
			lista.Sort((a, b) => b.priority.CompareTo(a.priority));

			foreach (var item in lista)
			{
				if (MatchingItems.Exists((a) => a.HyPixel_ID == item.item_dictKey))
				{
					var smth = MatchingItems.Find((a) => a.HyPixel_ID == item.item_dictKey);
					if (!item.playSound) return smth;
					PlaySound(smth.uuid);
					return smth;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Saves Recipes from RAM to Config for later use
		/// </summary>
		public void SaveRecipes()
		{
			ItemsToSearchForCatalogues.ForEach((a) => a.Items.RemoveAll((b) => b.item_dictKey == null));
			MainViewModel.Settings.Default.BetterAH_ItemsToSearchForCatalogues = ItemsToSearchForCatalogues;
			MainViewModel.Settings.Save();
			ReloadRecipes();
		}

		/// <summary>
		/// Loads Recipes from Config (Not saved changes will perish)
		/// </summary>
		public void LoadRecipes()
		{
			MainViewModel.Settings.Load();
			ItemsToSearchForCatalogues = MainViewModel.Settings.Default.BetterAH_ItemsToSearchForCatalogues;
			ReloadRecipes();
		}
		public async Task LoadRecipesAsync()
		{
			MainViewModel.Settings.Load();
			ItemsToSearchForCatalogues = MainViewModel.Settings.Default.BetterAH_ItemsToSearchForCatalogues;
			await ReloadRecipesAsync();
		}

		/// <summary>
		/// Finds matching items again for the instanced query <br/>
		/// Does not Load/Save to hard drive
		/// </summary>
		public void ReloadRecipes()
		{
			FindAllMatchingItems();
		}

		public async Task ReloadRecipesAsync()
		{
			await FindAllMatchingItemsAsync();
		}

		/// <summary>
		/// Searches Query List for specific item with given recipe_key
		/// </summary>
		/// <param name="recipe_key">ItemToSearchFor unique id</param>
		/// <returns></returns>
		public ItemToSearchFor GetRecipe(string recipe_key)
		{
			return ItemsToSearchFor.Find((a) => a.recipe_key == recipe_key);
		}

		/// <summary>
		/// Assigns recipe_key and adds Query to list
		/// </summary>
		/// <param name="searchQuery">Query to be added to list</param>
		public void AddRecipe(ItemToSearchFor searchQuery)
		{
			if (searchQuery == null) return;
			var query = new ItemToSearchFor(searchQuery);
			query.recipe_key = AssignNewUniqueKey(query.item_dictKey);
			ItemsToSearchFor.Add(query);
			ReloadRecipes();
		}
		public void MoveRecipe(ItemToSearchFor item, FlowDirection flowDirection)
		{
			if (item == null) return;
			if (!ItemsToSearchFor.Contains(item)) return;
			var index = ItemsToSearchFor.IndexOf(item);
			switch (flowDirection)
			{
				case FlowDirection.LeftToRight:
					if (index >= ItemsToSearchFor.Count - 1) return;
					ItemsToSearchFor.RemoveAt(index);
					ItemsToSearchFor.Insert(index + 1, item);
					break;
				case FlowDirection.RightToLeft:
					if (index <= 0) return;
					ItemsToSearchFor.RemoveAt(index);
					ItemsToSearchFor.Insert(index - 1, item);
					break;
			}
			ReloadRecipes();
		}

		public void RemoveRecipe(string recipe_key)
		{
			ItemsToSearchFor.RemoveAll((a) => a.recipe_key.Equals(recipe_key));
			ReloadRecipes();
		}

		/// <summary>
		/// Assigns new unique recipe_key from item's hypixel_ID
		/// </summary>
		/// <param name="item_dictKey">Item's hypixel_ID</param>
		/// <returns></returns>
		private string AssignNewUniqueKey(string item_dictKey)
		{
			string str = new(item_dictKey);
			str += ':';
			int i = new();
			while (i < ItemsToSearchFor.Count)
			{
				var tempstr = str + i.ToString();
				if (ItemsToSearchFor.Exists((a) => a.recipe_key == tempstr)) i++;
				else break;
			}

			str += i.ToString();

			return str;
		}

		public Auction FindCheapestMatchingItem(ItemToSearchFor toMatch)
		{
			if (toMatch.item_dictKey == null) return null;
			if (!MainViewModel.auctionHouse.auctions.TryGetValue(toMatch.item_dictKey, out var itemsToSearchOn)) { return null; }

			itemsToSearchOn.Sort((a, b) => a.starting_bid.CompareTo(b.starting_bid));

			foreach (var entry in itemsToSearchOn)
			{
				if (!entry.bin) continue;                           //Skip if not bin
				bool doesMatch = true;
				//Skip if lore doesn't contain text

				foreach (var text in toMatch.searchQueries)
				{
					if (!entry.item_lore.Contains(text)) { doesMatch = false; break; }
				}
				if (!doesMatch) continue;
				return entry;
			}
			return null;
		}

		private void PlaySound(string uuid)
		{
			if (highlitedAuction_uuid == uuid) { return; }
			highlitedAuction_uuid = uuid;
			soundPlayer.Play();
			if (MainViewModel.jsonStruct.Copy) CopyToClipboard.Copy("/viewauction " + uuid);
		}
	}
}

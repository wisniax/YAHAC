using ITR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Ink;
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
		public List<ItemToSearchFor> ItemsToSearchFor { get; private set; }
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
			if (ItemsToSearchFor == null) ItemsToSearchFor = new();
			MainViewModel.auctionHouse.AHUpdatedEvent += AuctionHouse_Updated;
		}

		private void AuctionHouse_Updated(AuctionHouse source)
		{
			if (source == null) return;
			if (!source.success) return;
			findMatchingItems();
			FindHighestPriorityAuction();
		}

		/// <summary>
		/// Executed everytime refresh succeded
		/// </summary>
		private void OnBetterAHUpdated()
		{
			BetterAHUpdatedEvent?.Invoke(this);
		}

		void findMatchingItems()
		{   //THIS WAY FINDING ITEMS IS 12 times faster for me... Wonder whyy (Totally not 12 threads CPU)
			List<Auction> tempmatchingItems = new();
			if (ItemsToSearchFor.Count == 0) { success = false; return; }
			var tasks = new List<Task>();
			foreach (var item in ItemsToSearchFor)
			{
				if (!item.enabled) continue;
				tasks.Add(Task.Run(() => checkIfItemsMatch(item, tempmatchingItems)));
			}
			Task.WaitAll(tasks.ToArray());
			tempmatchingItems.Sort((a, b) => a.starting_bid.CompareTo(b.starting_bid));
			MatchingItems = tempmatchingItems;
			lastCalculated = MainViewModel.auctionHouse.lastUpdated;
			success = true;
			OnBetterAHUpdated();
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
			var lista = ItemsToSearchFor.FindAll((a) => a.priority >= 0);
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
			MainViewModel.settings.Default.BetterAH_Query = ItemsToSearchFor;
			MainViewModel.settings.Save();
			findMatchingItems();
		}

		/// <summary>
		/// Loads Recipes from Config (Not saved changes will perish)
		/// </summary>
		public void LoadRecipes()
		{
			MainViewModel.settings.Load();
			ItemsToSearchFor = MainViewModel.settings.Default.BetterAH_Query;
			findMatchingItems();
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
			findMatchingItems();
		}

		public void RemoveRecipe(string recipe_key)
		{
			ItemsToSearchFor.RemoveAll((a) => a.recipe_key.Equals(recipe_key));
			findMatchingItems();
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
			var matchingDictKeys = ItemsToSearchFor.FindAll((a) => a.item_dictKey == item_dictKey);
			int i = new();
			while (i < matchingDictKeys.Count)
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

		void PlaySound(string uuid)
		{
			if (highlitedAuction_uuid == uuid) { return; }
			highlitedAuction_uuid = uuid;
			soundPlayer.Play();
			CopyToClipboard.Copy("/viewauction " + uuid);
		}
	}
}

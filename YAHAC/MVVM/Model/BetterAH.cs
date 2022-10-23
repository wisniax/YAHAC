using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Ink;
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

		public delegate void BetterAHUpdatedHandler(BetterAH source);
		public event BetterAHUpdatedHandler BetterAHUpdatedEvent;

		public BetterAH()
		{
			success = false;
			MatchingItems = new();
			highlitedAuction_uuid = new("");
			loadRecipes();
			if (ItemsToSearchFor == null) ItemsToSearchFor = new();
			MainViewModel.auctionHouse.AHUpdatedEvent += AuctionHouse_Updated;
		}

		private void AuctionHouse_Updated(AuctionHouse source)
		{
			if (source == null) return;
			if (!source.success) return;
			findMatchingItems();
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

		public Auction GetHighestPriorityAuction()
		{
			var lista = ItemsToSearchFor.FindAll((a) => a.priority >= 1);
			lista.Sort((a, b) => b.priority.CompareTo(a.priority));

			foreach (var item in lista)
			{
				if (MatchingItems.Exists((a) => a.HyPixel_ID == item.item_dictKey))
				{
					var smth = MatchingItems.Find((a) => a.HyPixel_ID == item.item_dictKey);
					return smth;
				}
			}
			return null;
		}

		public void saveRecipes()
		{
			MainViewModel.settings.Default.BetterAH_Query = ItemsToSearchFor;
			MainViewModel.settings.Save();
		}

		public void loadRecipes()
		{
			ItemsToSearchFor = MainViewModel.settings.Default.BetterAH_Query;
			return;
		}

		public void addRecipe(ItemToSearchFor searchQuery)
		{
			//var cus = new ItemToSearchFor();
			//cus.priority = 0;
			//cus.searchQueries = new();
			//cus.maxPrice = 1500000000;
			//cus.recipe_key = "HYPERION:1";
			//cus.item_dictKey = "HYPERION";
			ItemsToSearchFor.Add(searchQuery);
		}
	}
}

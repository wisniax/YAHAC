using ITR;
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
			LoadRecipes();
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

		/// <summary>
		/// Saves Recipes from RAM to Config for later use
		/// </summary>
		public void SaveRecipes()
		{
			MainViewModel.settings.Default.BetterAH_Query = ItemsToSearchFor;
			MainViewModel.settings.Save();
		}

		/// <summary>
		/// Loads Recipes from Config (Not saved changes will perish)
		/// </summary>
		public void LoadRecipes()
		{
			ItemsToSearchFor = MainViewModel.settings.Default.BetterAH_Query;
			return;
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
			searchQuery.recipe_key = AssignNewUniqueKey(searchQuery.item_dictKey);


			ItemsToSearchFor.Add(searchQuery);
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

		//To be Rewritten
		/*public partial class AddItemUC : UserControl
	{
		List<BetterAH.ItemToSearchFor> itemsToSearchFor;
		BetterAH.ItemToSearchFor newItem;
		public AddItemUC()
		{
			newItem = new();
			newItem.searchQueries = new();
			try
			{
				itemsToSearchFor = loadRecipes();
				if (itemsToSearchFor == null) throw new Exception();
			}
			catch (Exception e)
			{
				itemsToSearchFor = new();
			}
			InitializeComponent();
			generateComboItemToCraftList();
			comboBoxAddItemToRecipe.DisplayMember = "name";
			comboBoxAddItemToRecipe.ValueMember = "id";
			comboBoxAddItemToRecipe.DataSource = Properties.AllItemsREPO.itemRepo.items;

		}

		///
		///		METHODS
		///

		//Generates combo box responsible for selecting stored item recipes
		private void generateComboItemToCraftList(string selectKey = null)
		{
			comboBoxItemToCraft.Items.Clear();
			var recipePairs = new List<ItemRecipePair>();
			var addNewItem = new ItemRecipePair { item_name = "Add new item", item_dictKey = "Add new item", recipe_key = "Add new item" };
			recipePairs.Add(addNewItem);
			if (itemsToSearchFor != null)
			{
				foreach (var item in itemsToSearchFor)
				{
					var recipePair = new ItemRecipePair { item_name = Properties.AllItemsREPO.IDtoNAME(item.item_dictKey), item_dictKey = item.item_dictKey, recipe_key = item.recipe_key };
					if ((recipePair.recipe_key == selectKey) && (selectKey != null)) addNewItem = recipePair;
					recipePairs.Add(recipePair);
				}
			}
			comboBoxItemToCraft.DisplayMember = "item_name";
			comboBoxItemToCraft.ValueMember = "item_dictKey";
			foreach (var item in recipePairs)
			{
				comboBoxItemToCraft.Items.Add(item);
			}
			comboBoxItemToCraft.SelectedItem = addNewItem;
			buttonRemoveWholeSelectedItem.Enabled = false;
			buttonSaveItem.Enabled = false;
			buttonAddToItemReqList.Enabled = true;
			numericUpDownMaxPrice.Enabled = false;
			numericUpDownItemPriority.Enabled = false;
			textBoxRecipe.Clear();
		}

		//Generates recipe of provided itemRecipe on to textBox
		private void genRecipeInTextBox(BetterAH.ItemToSearchFor itemRecipo)
		{
			textBoxRecipe.Clear();
			if (itemRecipo.searchQueries == null) return;
			foreach (var item in itemRecipo.searchQueries)
			{
				textBoxRecipe.AppendText(item + Environment.NewLine); //New line is "\r\n"
			}
		}

		///
		///		BUTTON CLICKS
		///


		private class ItemRecipePair
		{
			public string item_name { get; set; }
			public string item_dictKey { get; set; }
			public string recipe_key { get; set; }
		}

		private void buttonSaveItem_Click_1(object sender, EventArgs e)
		{
			newItem.maxPrice = (uint)numericUpDownMaxPrice.Value;
			newItem.priority = (ushort)numericUpDownItemPriority.Value;
			if (!itemsToSearchFor.Exists((a) => a.recipe_key == newItem.recipe_key))
			{
				itemsToSearchFor.Add(newItem);
			}

			//if (newItem.id==0) newItem.id = itemsToSearchFor.Count((a) => a.item_dictKey == newItem.item_dictKey);

			newItem = new();
			newItem.searchQueries = new();
			saveRecipes();
			generateComboItemToCraftList();
		}

		private void buttonAddToItemReqList_Click_1(object sender, EventArgs e)
		{
			//Check whether we generate new item recipe or add req items to already existing one
			if (((ItemRecipePair)comboBoxItemToCraft.SelectedItem).item_dictKey == "Add new item")
			{
				if (!Properties.AllItemsREPO.itemRepo.items.Contains((Properties.AllItemsREPO.Item)comboBoxAddItemToRecipe.SelectedItem)) return;
				newItem.item_dictKey = ((Properties.AllItemsREPO.Item)comboBoxAddItemToRecipe.SelectedItem).id;
				newItem.recipe_key = assignNewUniqueKey(newItem.item_dictKey);
				var itemek = new ItemRecipePair { item_name = Properties.AllItemsREPO.IDtoNAME(newItem.item_dictKey), item_dictKey = newItem.item_dictKey, recipe_key = newItem.recipe_key };
				comboBoxItemToCraft.Items.Add(itemek);
				buttonRemoveWholeSelectedItem.Enabled = true;
				buttonSaveItem.Enabled = true;
				buttonAddToItemReqList.Enabled = true;
				numericUpDownMaxPrice.Enabled = true;
				numericUpDownItemPriority.Enabled = true;
				comboBoxItemToCraft.SelectedItem = itemek;
			}
			else
			{
				buttonSaveItem.Enabled = true;
				string str = comboBoxAddItemToRecipe.Text;
				newItem.searchQueries.Add(str);
				genRecipeInTextBox(newItem);
			}
		}

		private void buttonRemoveWholeSelectedItem_Click_1(object sender, EventArgs e)
		{
			itemsToSearchFor.Remove(newItem);
			newItem = new();
			newItem.searchQueries = new();
			saveRecipes();
			generateComboItemToCraftList();
		}

		///
		///		EVENTS
		///

		private void comboBoxItemToCraft_SelectionChangeCommitted_1(object sender, EventArgs e)
		{
			if (((System.Windows.Forms.ComboBox)sender).Items.Count == 0) return;
			var selectedPair = (ItemRecipePair)comboBoxItemToCraft.SelectedItem;
			if (selectedPair.item_dictKey == "Add new item")
			{
				newItem = new();
				newItem.searchQueries = new();
				//numericUpDownItemPriority.Value = newItem.priority;
				//numericUpDownMaxPrice.Value = newItem.maxPrice;
				buttonRemoveWholeSelectedItem.Enabled = false;
				buttonSaveItem.Enabled = false;
				buttonAddToItemReqList.Enabled = true;
				numericUpDownMaxPrice.Enabled = true;
				numericUpDownItemPriority.Enabled = true;
				generateComboItemToCraftList();
			}
			else
			{
				newItem = itemsToSearchFor.Find(a => (a.recipe_key == selectedPair.recipe_key) && a.recipe_key != null);

				if (newItem == null)
				{
					newItem = itemsToSearchFor.Find(a => (a.item_dictKey == selectedPair.item_dictKey) && a.recipe_key == null);
					newItem.recipe_key = assignNewUniqueKey(selectedPair.item_dictKey);
					saveRecipes();
					generateComboItemToCraftList(newItem.recipe_key);
				}

				if (newItem.searchQueries == null) newItem.searchQueries = new();
				numericUpDownItemPriority.Value = newItem.priority;
				numericUpDownMaxPrice.Value = newItem.maxPrice;
				buttonRemoveWholeSelectedItem.Enabled = true;
				buttonSaveItem.Enabled = true;
				buttonAddToItemReqList.Enabled = true;
				numericUpDownMaxPrice.Enabled = true;
				numericUpDownItemPriority.Enabled = true;
				genRecipeInTextBox(newItem);
			}
		}
	}*/
	}
}

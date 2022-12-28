using ITR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using YAHAC.Core;
using YAHAC.MVVM.UserControls;

namespace YAHAC.MVVM.ViewModel
{
	internal class AuctionHouseViewModel : ObservableObject
	{

		public List<string> ItemsInObservableCollection { get; set; }

		private ObservableCollection<object> _Items;
		public ObservableCollection<object> Items
		{
			get { return _Items; }
			set
			{
				_Items = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollection<object> HiddenItems;

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

		private Item _SelectedItem;
		public Item SelectedItem
		{
			get { return _SelectedItem; }
			set
			{
				if (value == null) { AdditionalInfo_Visible = false; return; }
				_SelectedItem = value;
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

		private string _SearchQuery;
		public string SearchQuery
		{
			get { return _SearchQuery; }
			set
			{
				_SearchQuery = value;
				OnPropertyChanged();
				OnSearchQueryChanged();
			}
		}


		public AuctionHouseViewModel()
		{
			Items = new();
			HiddenItems = new();
			SearchQuery = new("");
			ItemsInObservableCollection = new();
			MainViewModel.auctionHouse.AHUpdatedEvent += AuctionHouse_Updated;
			if (Items.Count == 0 && HiddenItems.Count == 0 && MainViewModel.auctionHouse.success) LoadAuctionHouse();
		}

		// Sound too good to be true lmao https://stackoverflow.com/questions/53549710/async-loading-usercontrols-in-itemcontrol-wpf-c-sharp
		async Task LoadAuctionHouse()
		{
			List<DispatcherOperation> operations = new(MainViewModel.auctionHouse.auctions.Count);

			//await Task.Run(() => (MainViewModel.bazaar.success == true));
            var sw = Stopwatch.StartNew();
			foreach (var key in MainViewModel.auctionHouse.auctions.Keys)
			{

				if (key == null) continue;
				if (ItemsInObservableCollection.Contains(key)) continue;

				var item = MainViewModel.itemTextureResolver.GetItemFromID(key);
                //if (item == null) continue;
                // In case item does not exist in Hypixel API create an unknown one with id as its name
                
                var operation = Application.Current.Dispatcher.BeginInvoke(
					DispatcherPriority.Background,
                    () =>
					{
                        item ??= new Item(
                          key,
                          key,
                          Material.AIR,
                          true,
                          MainViewModel.NoTextureMarkItem);

                        ItemView itemBox = new(item);
						ItemsInObservableCollection.Add(key);
						Items?.Add(itemBox);
					}
				);

				operations.Add(operation);
            }

			foreach (var op in operations)
				await op;

            sw.Stop();
            Debug.WriteLine($"Loading AH took: {sw.ElapsedMilliseconds} ms");
			OnSearchQueryChanged();
		}

		private void AuctionHouse_Updated(Model.AuctionHouse source)
		{
				
			if (source == null) return;
			if (!source.success) return;
			LoadAuctionHouse();
			return;
				
		}

		/// <summary>
		/// Search engine :)
		/// </summary>
		void OnSearchQueryChanged()
		{
			//Deleting stuff from Items list
			var charSearchQUery = SearchQuery.ToCharArray();
			foreach (var obj in Items)
			{
				var item = obj as ItemView;
				int LastIndex = 0;

				bool FullListMatched = true;

				for (int i = 0; i < charSearchQUery.Length; i++)
				{
					var str = item.item.Name.Substring(LastIndex);
					var ind = str.IndexOf(charSearchQUery[i], StringComparison.OrdinalIgnoreCase);
					if (ind < 0) { FullListMatched = false; break; }
					//So that repeating characters have correct search
					LastIndex += ind + 1;
				}
				if (FullListMatched) continue;
				HiddenItems.Add(obj);
			}
			foreach (var obj in HiddenItems)
			{
				Items.Remove(obj);
			}


			//Readding stuff to Items list
			foreach (var obj in HiddenItems)
			{
				var item = obj as ItemView;
				int LastIndex = 0;

				bool FullListMatched = true;

				for (int i = 0; i < charSearchQUery.Length; i++)
				{
					var str = item.item.Name.Substring(LastIndex);
					var ind = str.IndexOf(charSearchQUery[i], StringComparison.OrdinalIgnoreCase);
					if (ind < 0) { FullListMatched = false; break; }
					//So that repeating characters have correct search
					LastIndex += ind + 1;
				}
				if (!FullListMatched) continue;
				Items.Add(obj);
			}
			foreach (var obj in Items)
			{
				HiddenItems.Remove(obj);
			}

			//Sorting maybe in future bc... https://docs.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-sort-and-group-data-using-a-view-in-xaml?view=netframeworkdesktop-4.8 this shiet
			//Items = Items.OrderBy((a) => (a as ItemView).item.Name);
		}
	}
}

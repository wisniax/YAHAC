using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using YAHAC.Core;
using YAHAC.MVVM.View;
using ITR;
using YAHAC.MVVM.UserControls;
using System.Windows;
using System.Globalization;

namespace YAHAC.MVVM.ViewModel
{
	internal class BazaarViewModel : ObservableObject
	{
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


		public BazaarViewModel()
		{
			Items = new();
			HiddenItems = new();
			SearchQuery = new("");
			MainViewModel.bazaar.BazaarUpdatedEvent += Bazaar_Downloaded;
		}

		private void Bazaar_Downloaded(Model.Bazaar source)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (source == null) return;
					if (!source.success) return;
					LoadBazaar();
					MainViewModel.bazaar.BazaarUpdatedEvent -= Bazaar_Downloaded;
					return;
				});
			}
			else
			{
				if (source == null) return;
				if (!source.success) return;
				LoadBazaar();
				MainViewModel.bazaar.BazaarUpdatedEvent -= Bazaar_Downloaded;
				return;
			}
		}


		void LoadBazaar()
		{
			//await Task.Run(() => (MainViewModel.bazaar.success == true));
			Items = new();
			foreach (var key in MainViewModel.bazaar.products.Keys)
			{
				if (key == null) continue;
				var item = MainViewModel.itemTextureResolver.GetItemFromID(key);
				//if (item == null) continue;
				Converters.BitmapToMemoryStream convbtm = new Converters.BitmapToMemoryStream();
				if (item == null)
					item = new Item(
						  key,
						  key,
						  Material.AIR,
						  true,
						  convbtm.Convert(Properties.Resources.NoTextureMark, null, null, CultureInfo.CurrentCulture) as MemoryStream);
				ItemView itemBox = new(item);
				Items?.Add(itemBox);
			}
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

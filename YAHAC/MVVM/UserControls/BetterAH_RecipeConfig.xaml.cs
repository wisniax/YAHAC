using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YAHAC.MVVM.Model;
using YAHAC.MVVM.ViewModel;
using YAHAC.Properties;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for BetterAH_RecipeConfig.xaml
	/// </summary>
	public partial class BetterAH_RecipeConfig : UserControl
	{

		public object itemToSearchFor
		{
			get { return (object)GetValue(itemToSearchForProperty); }
			set { SetValue(itemToSearchForProperty, value); }
		}

		// Using a DependencyProperty as the backing store for itemToSearchFor.  This enables animation, styling, binding, etc...
		//https://stackoverflow.com/questions/25989018/wpf-usercontrol-twoway-binding-dependency-property
		public static readonly DependencyProperty itemToSearchForProperty =
			DependencyProperty.Register("itemToSearchFor", typeof(object), typeof(BetterAH_RecipeConfig), new FrameworkPropertyMetadata(
			null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDependencyChanged));

		private static void OnDependencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is not BetterAH_RecipeConfig cfg) return;
			switch (cfg.itemToSearchFor)
			{
				case null:
					cfg.visibile = false; return;
				case ItemToSearchFor itemToSearchFor:
					{
						cfg.CatalogueName_TextBox.Visibility = Visibility.Collapsed;
						cfg.itemToSearchForConfigDockPanel.Visibility = Visibility.Visible;
						cfg.visibile = true;
						cfg.SearchQueries = itemToSearchFor.searchQueries;
						if (!cfg.AuctionableItems.Contains(itemToSearchFor.item_dictKey))
							cfg.AuctionableItems.Add(itemToSearchFor.item_dictKey);
						cfg.SelectedAuctionableItem = itemToSearchFor.item_dictKey;
						break;
					}
				case ItemsToSearchForCatalogue cata:
					{
						cfg.visibile = true;
						cfg.CatalogueName_TextBox.Visibility = Visibility.Visible;
						cfg.itemToSearchForConfigDockPanel.Visibility = Visibility.Collapsed;
						cfg.CatalogueName_TextBox.Text = cata.Name;
						cfg.SelectedAuctionableItem = null;
						cfg.SearchQueries = null;
						break;
					}
			}
		}

		public bool visibile
		{
			get { return (bool)GetValue(visibileProperty); }
			set { SetValue(visibileProperty, value); }
		}

		// Using a DependencyProperty as the backing store for visibile.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty visibileProperty =
			DependencyProperty.Register("visibile", typeof(bool), typeof(BetterAH_RecipeConfig), new PropertyMetadata(false));


		public List<string> SearchQueries
		{
			get { return (List<string>)GetValue(SearchQueriesProperty); }
			set { SetValue(SearchQueriesProperty, value); }
		}

		// Using a DependencyProperty as the backing store for searchQueries.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SearchQueriesProperty =
			DependencyProperty.Register("SearchQueries", typeof(List<string>), typeof(BetterAH_RecipeConfig), new PropertyMetadata(null, OnSearchQueriesChanged));

		private static void OnSearchQueriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BetterAH_RecipeConfig cfg = d as BetterAH_RecipeConfig;
			if (cfg?.itemToSearchFor is not ItemToSearchFor cfgItemToSearchFor) return;
			cfgItemToSearchFor.searchQueries = cfg.SearchQueries;
		}



		public ObservableCollection<object> AuctionableItems
		{
			get { return (ObservableCollection<object>)GetValue(AuctionableItemsProperty); }
			set { SetValue(AuctionableItemsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for AuctionableItems.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AuctionableItemsProperty =
			DependencyProperty.Register("AuctionableItems", typeof(ObservableCollection<object>), typeof(BetterAH_RecipeConfig), new PropertyMetadata(null));



		public object SelectedAuctionableItem
		{
			get { return (object)GetValue(SelectedAuctionableItemProperty); }
			set { SetValue(SelectedAuctionableItemProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SelectedAuctionableItem.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedAuctionableItemProperty =
			DependencyProperty.Register("SelectedAuctionableItem", typeof(object), typeof(BetterAH_RecipeConfig), new PropertyMetadata(null, OnSelectedAuctionableItemChanged));

		/// <summary>
		/// Changes itemToSearchFor.dictKey everytime letter in ComboBox is pressed...
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnSelectedAuctionableItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var cfg = (BetterAH_RecipeConfig)d;
			if (cfg?.SelectedAuctionableItem is not string str) return;
			if (cfg.itemToSearchFor is not ItemToSearchFor cfgItemToSearchFor) return;
			if (str == cfgItemToSearchFor.item_dictKey) return;
			cfgItemToSearchFor.item_dictKey = str;
		}


		//Someday https://www.codeproject.com/Articles/44920/A-Reusable-WPF-Autocomplete-TextBox or https://www.codeproject.com/Articles/31947/WPF-AutoComplete-Folder-TextBox
		public BetterAH_RecipeConfig()
		{
			AuctionableItems = new ObservableCollection<object>();
			MainViewModel.auctionHouse.AHUpdatedEvent += AH_Updated;
			InitializeComponent();
		}

		private void AH_Updated(AuctionHouse source)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (source == null) return;
					if (!source.success) return;
					ObservableCollection<object> tmp = new(source.auctions.Keys);
					if (itemToSearchFor is ItemToSearchFor cfgItemToSearchFor)
						if (!tmp.Contains(cfgItemToSearchFor.item_dictKey)) tmp.Add(cfgItemToSearchFor.item_dictKey);
					AuctionableItems = tmp;
					return;
				});
			}
			else
			{
				if (source == null) return;
				if (!source.success) return;
				ObservableCollection<object> tmp = new(source.auctions.Keys);
				if (itemToSearchFor is ItemToSearchFor cfgItemToSearchFor)
					if (!tmp.Contains(cfgItemToSearchFor.item_dictKey)) tmp.Add(cfgItemToSearchFor.item_dictKey);
				AuctionableItems = tmp;
				return;
			}
		}

		private void Close_Btn_Click(object sender, RoutedEventArgs e)
		{
			visibile = false;
		}

		private void AddToQuery_Textbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter) return;
			var textbox = sender as TextBox;
			if (textbox == null) return;
			if (SearchQueries == null) SearchQueries = new();
			List<string> cusie = new(SearchQueries);
			cusie.Add(textbox.Text);
			SearchQueries = cusie;
		}

		private void SearchQueries_List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var list = sender as ListView;
			if (list == null) return;
			var str = list.SelectedItem as string;
			if (str == null) return;
			List<string> cusie = new(SearchQueries);
			cusie.Remove(str);
			SearchQueries = cusie;
		}
		public void HideControl()
		{
			visibile = false;
		}

		private async void AuctionableItems_ComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter) return;
			var str = SelectedAuctionableItem as string;
			if (str == null) return;
			//if (str == itemToSearchFor.item_dictKey) return;
			//itemToSearchFor.item_dictKey = str;
			await MainViewModel.betterAH.ReloadRecipesAsync();
		}

		private void CatalogueName_TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter) return;
			if (sender is not TextBox textbox || itemToSearchFor is not ItemsToSearchForCatalogue cata) return;
			MainViewModel.betterAH.RenameCatalogue(cata, textbox.Text);
			//if (str == itemToSearchFor.item_dictKey) return;
			//itemToSearchFor.item_dictKey = str;
		}
	}
}

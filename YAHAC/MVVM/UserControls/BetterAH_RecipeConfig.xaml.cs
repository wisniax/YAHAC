using System;
using System.Collections.Generic;
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
using YAHAC.Properties;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for BetterAH_RecipeConfig.xaml
	/// </summary>
	public partial class BetterAH_RecipeConfig : UserControl
	{

		public ItemToSearchFor itemToSearchFor
		{
			get { return (ItemToSearchFor)GetValue(itemToSearchForProperty); }
			set { SetValue(itemToSearchForProperty, value); }
		}

		// Using a DependencyProperty as the backing store for itemToSearchFor.  This enables animation, styling, binding, etc...
		//https://stackoverflow.com/questions/25989018/wpf-usercontrol-twoway-binding-dependency-property
		public static readonly DependencyProperty itemToSearchForProperty =
			DependencyProperty.Register("itemToSearchFor", typeof(ItemToSearchFor), typeof(BetterAH_RecipeConfig), new FrameworkPropertyMetadata(
			null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDependencyChanged));

		private static void OnDependencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			BetterAH_RecipeConfig cfg = d as BetterAH_RecipeConfig;
			if (cfg == null) return;
			if (cfg.itemToSearchFor == null) { cfg.visibile = false; return; }
			cfg.visibile = true;
			cfg.SearchQueries = cfg.itemToSearchFor.searchQueries;
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
			if (cfg == null) return;
			if (cfg.itemToSearchFor == null) return;
			cfg.itemToSearchFor.searchQueries = cfg.SearchQueries;
		}





		//Someday https://www.codeproject.com/Articles/44920/A-Reusable-WPF-Autocomplete-TextBox or https://www.codeproject.com/Articles/31947/WPF-AutoComplete-Folder-TextBox
		public BetterAH_RecipeConfig()
		{
			InitializeComponent();
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
			//SearchQueries.Add(textbox.Text);
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
	}
}

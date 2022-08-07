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
using System.Windows.Threading;
using ITR;
using YAHAC.Core;
using YAHAC.MVVM.Model;
using YAHAC.MVVM.ViewModel;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for BazaarItemPeek.xaml
	/// </summary>
	public partial class BazaarItemPeek : UserControl
	{
		public string SelectedItemID
		{
			get { return (string)GetValue(SelectedItemIDProperty); }
			set
			{
				if (value == null) return;
				SetValue(SelectedItemIDProperty, value);
			}
		}
		// Using a DependencyProperty as the backing store for SelectedItemID.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedItemIDProperty =
			DependencyProperty.Register("SelectedItemID", typeof(string), typeof(BazaarItemPeek),
				new PropertyMetadata(OnDependencyChanged));

		public Item SelectedItem
		{
			get { return (Item)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(Item), typeof(BazaarItemPeek));

		public BazaarItemDef BazaarItemData
		{
			get { return (BazaarItemDef)GetValue(BazaarItemDataProperty); }
			set { SetValue(BazaarItemDataProperty, value); }
		}
		// Using a DependencyProperty as the backing store for BazaarItemData.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BazaarItemDataProperty =
			DependencyProperty.Register("BazaarItemData", typeof(BazaarItemDef), typeof(BazaarItemPeek));


		public BazaarItemPeek()
		{
			InitializeComponent();
			MainViewModel.bazaar.BazaarUpdatedEvent += Bazaar_BazaarUpdatedEvent;
		}

		public static void OnDependencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (!MainViewModel.bazaar.success) return;
			if (!MainViewModel.itemTextureResolver.Initialized) return;
			var BIP = (o as BazaarItemPeek);
			BIP.SelectedItem = MainViewModel.itemTextureResolver.GetItemFromID(BIP.SelectedItemID);
			BIP.BazaarItemData = MainViewModel.bazaar.GetBazaarItemDataFromID(BIP.SelectedItemID);
		}

		private void Bazaar_BazaarUpdatedEvent(Model.Bazaar source)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (source == null) return;
					if (!source.success) return;
					if (SelectedItem == null) return;
					BazaarItemData = MainViewModel.bazaar.GetBazaarItemDataFromID(SelectedItem.HyPixel_ID);
					return;
				});
			}
			else
			{
				if (source == null) return;
				if (!source.success) return;
				if (SelectedItem == null) return;
				BazaarItemData = MainViewModel.bazaar.GetBazaarItemDataFromID(SelectedItem.HyPixel_ID);
				return;
			}
		}

		private void RenderOffers()
		{
			//var cont = new BazaarOfferDsiplay();
			//cont.Offers = BazaarItemData.buy_summary;
			//OffersPanel.Children.Add(cont);
		}
	}
}

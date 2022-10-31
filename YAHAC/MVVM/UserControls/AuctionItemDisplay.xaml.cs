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
using YAHAC.MVVM.Model;
using YAHAC.MVVM.ViewModel;
using YAHAC.Properties;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for AuctionItemDisplay.xaml
	/// </summary>
	public partial class AuctionItemDisplay : UserControl
	{

		public Auction SelectedAuction
		{
			get { return (Auction)GetValue(SelectedAuctionProperty); }
			set { SetValue(SelectedAuctionProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SelectedAuction.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedAuctionProperty =
			DependencyProperty.Register("SelectedAuction", typeof(Auction), typeof(AuctionItemDisplay), new PropertyMetadata(OnDependencyChanged));


		public ItemToSearchFor ItemQuery
		{
			get { return (ItemToSearchFor)GetValue(ItemQueryProperty); }
			set { SetValue(ItemQueryProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ItemQuery.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ItemQueryProperty =
			DependencyProperty.Register("ItemQuery", typeof(ItemToSearchFor), typeof(AuctionItemDisplay), new PropertyMetadata(null));


		public long CheapestAuctionPrice
		{
			get { return (long)GetValue(CheapestAuctionPriceProperty); }
			set { SetValue(CheapestAuctionPriceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CheapestAuctionPrice.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CheapestAuctionPriceProperty =
			DependencyProperty.Register("CheapestAuctionPrice", typeof(long), typeof(AuctionItemDisplay), new PropertyMetadata(null));


		public AuctionItemDisplay()
		{
			CheapestAuctionPrice = 0;
			InitializeComponent();
		}

		public static void OnDependencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (!MainViewModel.betterAH.success) return;
			if (!MainViewModel.itemTextureResolver.Initialized) return;
			var AID = (o as AuctionItemDisplay);
			if (AID == null || AID.SelectedAuction == null) { AID.CheapestAuctionPrice = 0; return; }
			try
			{
				var cus = MainViewModel.auctionHouse.auctions[AID.SelectedAuction.HyPixel_ID];
				cus.Sort((a, b) => a.starting_bid.CompareTo(b.starting_bid));
				AID.CheapestAuctionPrice = cus[0].starting_bid;
			}
			catch (Exception)
			{
				AID.CheapestAuctionPrice = 0;
			}

		}
	}
}

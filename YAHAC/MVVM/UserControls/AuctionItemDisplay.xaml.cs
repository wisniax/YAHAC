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



		public AuctionItemDisplay()
		{
			InitializeComponent();
		}

		public static void OnDependencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (!MainViewModel.betterAH.success) return;
			if (!MainViewModel.itemTextureResolver.Initialized) return;
			var AID = (o as AuctionItemDisplay);
		}
	}
}

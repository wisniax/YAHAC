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
using YAHAC.Core;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for BazaarOfferDsiplay.xaml
	/// </summary>
	public partial class BazaarOfferDsiplay : UserControl
	{


		public List<DataPatterns.BzOrders> Offers
		{
			get { return (List<DataPatterns.BzOrders>)GetValue(OffersProperty); }
			set { SetValue(OffersProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Offers.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OffersProperty =
			DependencyProperty.Register("Offers", typeof(List<DataPatterns.BzOrders>), typeof(BazaarOfferDsiplay));


		public DataPatterns.BzOrders BzOffer
		{
			get { return (DataPatterns.BzOrders)GetValue(BzOfferProperty); }
			set { SetValue(BzOfferProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BzOffer.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BzOfferProperty =
			DependencyProperty.Register("BzOffer", typeof(DataPatterns.BzOrders), typeof(BazaarOfferDsiplay));





		public BazaarOfferDsiplay()
		{
			InitializeComponent();
		}
	}
}

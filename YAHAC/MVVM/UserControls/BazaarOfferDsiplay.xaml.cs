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
using YAHAC.MVVM.Model;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for BazaarOfferDsiplay.xaml
	/// </summary>
	public partial class BazaarOfferDsiplay : UserControl
	{


		public List<BzOrders> Offers
		{
			get { return (List<BzOrders>)GetValue(OffersProperty); }
			set { SetValue(OffersProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Offers.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OffersProperty =
			DependencyProperty.Register("Offers", typeof(List<BzOrders>), typeof(BazaarOfferDsiplay));


		public BzOrders BzOffer
		{
			get { return (BzOrders)GetValue(BzOfferProperty); }
			set { SetValue(BzOfferProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BzOffer.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BzOfferProperty =
			DependencyProperty.Register("BzOffer", typeof(BzOrders), typeof(BazaarOfferDsiplay));





		public BazaarOfferDsiplay()
		{
			InitializeComponent();
		}
	}
}

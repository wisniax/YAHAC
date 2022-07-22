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

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for BazaarOfferDsiplay.xaml
	/// </summary>
	public partial class BazaarOfferDsiplay : UserControl
	{
		public class KeyPair
		{
			public string Key { get; set; }
			public string Value { get; set; }
		}



		public KeyPair keyPair
		{
			get { return (KeyPair)GetValue(keyPairProperty); }
			set { SetValue(keyPairProperty, value); }
		}

		// Using a DependencyProperty as the backing store for _KeyPair.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty keyPairProperty =
			DependencyProperty.Register("keyPair", typeof(KeyPair), typeof(BazaarOfferDsiplay));



		public BazaarOfferDsiplay()
		{
			InitializeComponent();
		}
	}
}

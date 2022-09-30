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
using YAHAC.Converters;
using YAHAC.MVVM.UserControls;
using YAHAC.MVVM.ViewModel;

namespace YAHAC.MVVM.View
{
	/// <summary>
	/// Interaction logic for AuctionHouseView.xaml
	/// </summary>
	public partial class AuctionHouseView : UserControl
	{
		public AuctionHouseView()
		{
			InitializeComponent();
		}
		private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((AuctionHouseViewModel)DataContext).SelectedItem = ((ItemView)((ListBox)sender).SelectedItem)?.item;
		}

		private void UserControl_MouseMove(object sender, MouseEventArgs e)
		{
			MouseToCanvasOffset offs = new();
			((AuctionHouseViewModel)DataContext).CanvasPoint = (Point)offs.Convert(sender, typeof(BazaarView), e, null);
		}
	}
}

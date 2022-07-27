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
using YAHAC.MVVM.UserControls;
using YAHAC.MVVM.ViewModel;
using YAHAC.Converters;

namespace YAHAC.MVVM.View
{
	/// <summary>
	/// Interaction logic for BazaarView.xaml
	/// </summary>
	public partial class BazaarView : UserControl
	{
		public BazaarView()
		{
			InitializeComponent();
		}

		private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((BazaarViewModel)DataContext).SelectedItem = ((ItemView)((ListBox)sender).SelectedItem)?.item;
		}

		private void UserControl_MouseMove(object sender, MouseEventArgs e)
		{
			MouseToCanvasOffset offs = new();
			((BazaarViewModel)DataContext).CanvasPoint = (Point)offs.Convert(sender, typeof(BazaarView), e, null);
		}
    }
}

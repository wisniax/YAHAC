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
	/// Interaction logic for BetterAHView.xaml
	/// </summary>
	public partial class BetterAHView : UserControl
	{
		public BetterAHView()
		{
			InitializeComponent();
		}

		private void UserControl_MouseMove(object sender, MouseEventArgs e)
		{
			MouseToCanvasOffset offs = new();
			((BetterAHViewModel)DataContext).CanvasPoint = (Point)offs.Convert(sender, typeof(BetterAHView), e, null);
		}

		private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((BetterAHViewModel)DataContext).SelectedItemView = ((ItemView)((ListBox)sender).SelectedItem);
		}

		private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			((BetterAHViewModel)DataContext).MouseDoubleClicked(sender, e);
		}

		private void LoadConfig_Btn_Click(object sender, RoutedEventArgs e)
		{
			MainViewModel.betterAH.LoadRecipes();
		}

		private void SaveConfig_Btn_Click(object sender, RoutedEventArgs e)
		{
			MainViewModel.betterAH.SaveRecipes();
		}
    }
}

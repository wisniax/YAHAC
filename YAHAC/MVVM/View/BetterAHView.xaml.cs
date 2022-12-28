using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Windows.ApplicationModel.Contacts;
using YAHAC.Converters;
using YAHAC.MVVM.UserControls;
using YAHAC.MVVM.ViewModel;
using YAHAC.Properties;

namespace YAHAC.MVVM.View
{
	/// <summary>
	/// Interaction logic for BetterAHView.xaml
	/// </summary>
	public partial class BetterAHView : UserControl
	{
		private TaskCompletionSource<ItemsToSearchForCatalogue> tcs = new TaskCompletionSource<ItemsToSearchForCatalogue>();
		public BetterAHView()
		{
			InitializeComponent();
		}

		private void UserControl_MouseMove(object sender, MouseEventArgs e)
		{
			MouseToCanvasOffset offs = new();
			((BetterAhViewModel)DataContext).CanvasPoint = (Point)offs.Convert(sender, typeof(BetterAHView), e, null);
		}

		private void ItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((BetterAhViewModel)DataContext).SelectedItemView = ((sender as ListBox).SelectedItem as ItemView);
		}

		private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			((BetterAhViewModel)DataContext).MouseDoubleClicked(sender, e);
		}

		private void LoadConfig_Btn_Click(object sender, RoutedEventArgs e)
		{
			MainViewModel.betterAH.LoadRecipes();
			((BetterAhViewModel)DataContext).SelectedItemToRecipeConfig = null;
		}

		private void SaveConfig_Btn_Click(object sender, RoutedEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftShift)) MainViewModel.betterAH.HardSaveRecipes();
			else MainViewModel.betterAH.SaveRecipes();
			((BetterAhViewModel)DataContext).SelectedItemToRecipeConfig = null;
		}

		private static bool _isItemSelected = false;

		private async void ItemsToSearchForConfigurableList_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (_isItemSelected) return;
			if (sender is not ListBox list) return;
			var selectedItem = list.SelectedItem as ItemView;
			await Task.Delay(200);
			tcs = new TaskCompletionSource<ItemsToSearchForCatalogue>();
			if (Mouse.RightButton == MouseButtonState.Pressed && !_isItemSelected && selectedItem == list.SelectedItem)
			{
				MoveItemCanvas.Visibility = Visibility.Visible;
				_isItemSelected = true;
				var yoo = await tcs.Task;
				if (yoo == null || selectedItem?.itemToSearchFor == null)
				{
					MoveItemCanvas.Visibility = Visibility.Collapsed;
					_isItemSelected = false;
					return;
				}
				MainViewModel.betterAH.MoveItemToCatalogue(selectedItem.itemToSearchFor, yoo);
			}
			MoveItemCanvas.Visibility = Visibility.Collapsed;
			_isItemSelected = false;
		}

		private void UIElement_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!_isItemSelected) tcs.TrySetResult(null);
			else if (((BetterAhViewModel)DataContext).SelectedItemView.Tag is not ItemsToSearchForCatalogue cata || !((BetterAhViewModel)DataContext).AdditionalInfoVisible)
				tcs.TrySetResult(null);
			else tcs.SetResult(cata);
		}

		private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
		{
			UIElement_OnPreviewMouseRightButtonUp(sender, null);
		}
	}
}

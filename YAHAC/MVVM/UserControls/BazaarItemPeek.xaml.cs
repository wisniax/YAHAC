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
using ITR;
using YAHAC.Core;
using YAHAC.MVVM.ViewModel;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for BazaarItemPeek.xaml
	/// </summary>
	public partial class BazaarItemPeek : UserControl
	{
		private string _SelectedItemID;

		public string SelectedItemID
		{
			get { return _SelectedItemID == null ? "null" : _SelectedItemID; }
			set
			{
				_SelectedItemID = value;
				OnDependencyChanged();
			}
		}



		public DataPatterns.BazaarItemDef BazaarItemData
		{
			get { return (DataPatterns.BazaarItemDef)GetValue(BazaarItemDataProperty); }
			set { SetValue(BazaarItemDataProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BazaarItemData.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BazaarItemDataProperty =
			DependencyProperty.Register("BazaarItemData", typeof(DataPatterns.BazaarItemDef), typeof(BazaarItemPeek));







		async Task OnDependencyChanged()
		{
			await Task.Run(() => (MainViewModel.bazaar.success == true));
			BazaarItemData = MainViewModel.bazaar.GetBazaarItemDataFromID(SelectedItemID);
		}


		public BazaarItemPeek()
		{
			SelectedItemID = "CORRUPTED_BAIT";
			OnDependencyChanged();
			InitializeComponent();
		}
	}
}

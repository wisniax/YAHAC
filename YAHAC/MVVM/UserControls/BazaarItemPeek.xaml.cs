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
using System.Windows.Threading;
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
		public string SelectedItemID
		{
			get { return (string)GetValue(SelectedItemIDProperty); }
			set
			{
				if (value == null) return;
				SetValue(SelectedItemIDProperty, value);
			}
		}

		// Using a DependencyProperty as the backing store for SelectedItemID.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SelectedItemIDProperty =
			DependencyProperty.Register("SelectedItemID", typeof(string), typeof(BazaarItemPeek),
				new PropertyMetadata(BazaarItemPeekViewModel.OnDependencyChanged));



		public BazaarItemPeek()
		{
			InitializeComponent();
		}
	}
	public class BazaarItemPeekViewModel : ObservableObject
	{
		private Item _SelectedItem;
		public Item SelectedItem
		{
			get { return _SelectedItem; }
			set
			{
				_SelectedItem = value;
				OnPropertyChanged();
			}
		}

		private DataPatterns.BazaarItemDef _BazaarItemData;
		public DataPatterns.BazaarItemDef BazaarItemData
		{
			get { return _BazaarItemData; }
			set
			{
				_BazaarItemData = value;
				OnPropertyChanged();
			}
		}

		public BazaarItemPeekViewModel()
		{
			MainViewModel.bazaar.BazaarUpdatedEvent += Bazaar_BazaarUpdatedEvent;
		}


		public static void OnDependencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (!MainViewModel.bazaar.success) return;
			if (!MainViewModel.itemTextureResolver.Initialized) return;
			var BIP = (o as BazaarItemPeek);
			var BIPVM = BIP.DataContext as BazaarItemPeekViewModel;
			BIPVM.SelectedItem = MainViewModel.itemTextureResolver.GetItemFromID(BIP.SelectedItemID);
			BIPVM.BazaarItemData = MainViewModel.bazaar.GetBazaarItemDataFromID(BIP.SelectedItemID);
		}


		private void Bazaar_BazaarUpdatedEvent(Model.Bazaar source)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (source == null) return;
					if (!source.success) return;
					if (SelectedItem == null) return;
					BazaarItemData = MainViewModel.bazaar.GetBazaarItemDataFromID(SelectedItem.HyPixel_ID);
					return;
				});
			}
			else
			{
				if (source == null) return;
				if (!source.success) return;
				if (SelectedItem == null) return;
				BazaarItemData = MainViewModel.bazaar.GetBazaarItemDataFromID(SelectedItem.HyPixel_ID);
				return;
			}
		}
	}
}

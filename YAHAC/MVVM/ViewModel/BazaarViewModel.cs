using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using YAHAC.Core.ApiInstances;
using YAHAC.Core;
using YAHAC.MVVM.View;
using ITR;
using YAHAC.MVVM.UserControls;
using System.Windows;

namespace YAHAC.MVVM.ViewModel
{
	internal class BazaarViewModel : ObservableObject
	{
		private ObservableCollection<object> _Items;
		public ObservableCollection<object> Items
		{
			get { return _Items; }
			set
			{
				_Items = value;
				OnPropertyChanged();
			}
		}

		private bool _AdditonalInfo_Visible;
		public bool AdditionalInfo_Visible
		{
			get { return _AdditonalInfo_Visible; }
			set
			{
				_AdditonalInfo_Visible = value;
				OnPropertyChanged();
			}
		}

		private Item _SelectedItem;
		public Item SelectedItem
		{
			get { return _SelectedItem; }
			set
			{
				if (value == null) { AdditionalInfo_Visible = false; return; }
				_SelectedItem = value;
				OnPropertyChanged();
				AdditionalInfo_Visible = true;
			}
		}

		private Point _CanvasPoint;

		public Point CanvasPoint
		{
			get { return _CanvasPoint; }
			set
			{
				_CanvasPoint = value;
				OnPropertyChanged();
			}
		}



		public BazaarViewModel()
		{
			Items = new();
			_ = LoadBazaar();
		}

		async Task LoadBazaar()
		{
			await Task.Run(() => (MainViewModel.bazaar.success == true));
			foreach (var key in MainViewModel.bazaar.products.Keys)
			{
				var item = MainViewModel.itemTextureResolver.GetItemFromID(key);
				if (item == null) continue;
				ItemView itemBox = new(item);
				Items?.Add(itemBox);
			}
		}
	}
}

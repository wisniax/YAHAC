using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAHAC.Core;
using YAHAC.Core.ApiInstances;
using YAHAC.MVVM.View;

namespace YAHAC.MVVM.ViewModel
{
	internal class MainViewModel : ObservableObject
	{
		public RelayCommand BazaarViewCommand { get; set; }
		public RelayCommand AuctionHouseViewCommand { get; set; }
		BazaarViewModel BZView;
		private object _currentView;

		public object CurrentView
		{
			get { return _currentView; }
			set
			{
				_currentView = value;
				OnPropertyChanged();
			}
		}

		public MainViewModel()
		{
			if (Properties.Settings.Default.UpgradeRequired == true)
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.UpgradeRequired = false;
				Properties.Settings.Default.Save();
			}
			BazaarCheckup.refresh();
			BZView = new();
			BazaarViewCommand = new RelayCommand((o) => {  CurrentView = BZView.ViewModel; });
			AuctionHouseViewCommand = new RelayCommand((o) => { CurrentView = new AuctionHouseViewModel(); });
			BazaarViewCommand.Execute(this);
		}
	}
}

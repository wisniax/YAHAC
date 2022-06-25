using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAHAC.Core;
using YAHAC.MVVM.View;

namespace YAHAC.MVVM.ViewModel
{
	internal class MainViewModel : ObservableObject
	{
		public RelayCommand BazaarViewCommand { get; set; }
		public RelayCommand AuctionHouseViewCommand { get; set; }
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
			BazaarViewCommand = new RelayCommand((o) => { CurrentView = new BazaarViewModel().ViewModel; });
			AuctionHouseViewCommand = new RelayCommand((o) => { CurrentView = new AuctionHouseViewModel(); });
			BazaarViewCommand.Execute(this);
		}
	}
}

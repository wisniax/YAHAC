﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAHAC.Core;
using YAHAC.Properties;
using YAHAC.MVVM.View;
using YAHAC.MVVM.Model;
using System.Windows;
using System.ComponentModel;

namespace YAHAC.MVVM.ViewModel
{
	internal class MainViewModel : ObservableObject
	{
		//Thats how u bind a static property lmao
		static private Settings _settings;
		static public Settings settings
		{
			get { return _settings; }
			set { 
				_settings = value;
				settings_Changed();
			}
		}
		static public void settings_Changed()
		{
			StaticPropertyChanged?.Invoke(null, FilterStringPropertyEventArgs);
		}
		private static readonly PropertyChangedEventArgs FilterStringPropertyEventArgs = new PropertyChangedEventArgs(nameof(settings));
		public static event PropertyChangedEventHandler StaticPropertyChanged;
		//Commands
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

		//Models
		public static Bazaar bazaar;

		public MainViewModel()
		{
			bazaar = new();
			settings = new Settings();
			BZView = new();
			BazaarViewCommand = new RelayCommand((o) => { CurrentView = BZView.ViewModel; });
			AuctionHouseViewCommand = new RelayCommand((o) => { CurrentView = new AuctionHouseViewModel(); });
		}
	}
}

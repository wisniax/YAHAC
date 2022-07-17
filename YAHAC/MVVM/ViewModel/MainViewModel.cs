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
using ITR;

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
		public static ItemTextureResolver itemTextureResolver { get;private set; }
		//Commands
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

		public ItemsRepository itemsRepo { get; set; }

		//Models
		public static Bazaar bazaar;

		//Views
		BazaarView bazaarView;
		AuctionHouseView auctionHouseView;

		public MainViewModel()
		{
			itemsRepo = new();
			bazaar = new();
			settings = new();
			itemTextureResolver = new();
			itemTextureResolver.FastInit(Settings.SettingsPath + @"\ITR_Cache.zip");
			//itemTextureResolver.LoadResourcepack(Resources.FurfSkyRebornFULL);
			bazaarView = new();
			auctionHouseView = new();
			BazaarViewCommand = new RelayCommand((o) => { CurrentView = bazaarView; });
			AuctionHouseViewCommand = new RelayCommand((o) => { CurrentView = auctionHouseView; });
		}
	}
}

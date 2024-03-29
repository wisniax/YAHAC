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
using static YAHAC.Core.HypixelCertificateHandling;
using System.Globalization;
using System.IO;

namespace YAHAC.MVVM.ViewModel
{
	internal class MainViewModel : ObservableObject
	{
		//Thats how u bind a static property lmao
		private static Settings _settings;
		public static Settings Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				settings_Changed();
			}
		}
		public static JsonStruct jsonStruct { get; set; }
		public static MemoryStream NoTextureMarkItem { get; private set; }

		static public void settings_Changed()
		{
			StaticPropertyChanged?.Invoke(null, FilterStringPropertyEventArgs);
		}
		private static readonly PropertyChangedEventArgs FilterStringPropertyEventArgs = new PropertyChangedEventArgs(nameof(Settings));
		public static event PropertyChangedEventHandler StaticPropertyChanged;
		public static ItemTextureResolver itemTextureResolver { get; private set; }
		//Commands
		public RelayCommand BazaarViewCommand { get; set; }
		public RelayCommand AuctionHouseViewCommand { get; set; }
		public RelayCommand BetterAHViewCommand { get; set; }
		private object _currentView;
		public object CurrentView
		{
			get => _currentView;
			set
			{
				_currentView = value;
				OnPropertyChanged();
			}
		}


		//Models
		public static Bazaar bazaar;
		public static AuctionHouse auctionHouse;
		public static BetterAH betterAH;

		//Views
		BazaarView bazaarView;
		AuctionHouseView auctionHouseView;
		BetterAHView betterAHView;

		public MainViewModel()
		{
			itemTextureResolver = new();
			itemTextureResolver.FastInit(Settings.SettingsPath + @"\ITR_Cache.zip");
			Settings = new();
			jsonStruct = GetApiData();
			bazaar = new();
			auctionHouse = new();
			if (jsonStruct.Online) betterAH = new();
			var convbtm = new Converters.BitmapToMemoryStream();
			NoTextureMarkItem = convbtm.Convert(Properties.Resources.NoTextureMark, null, null, CultureInfo.CurrentCulture) as MemoryStream;

			//betterAH.addRecipe();
			//betterAH.saveRecipes();

			//ITR.ItemTextureResolver.HyItems_Item hyItem = new();
			//hyItem.id = Material.ENCHANTED_BOOK.ToString();
			//hyItem.material = hyItem.id;
			//itemTextureResolver.RegisterItem(hyItem, itemTextureResolver.GetItemFromID(Material.BREAD.ToString()).Texture);
			//itemTextureResolver.ResourcepackPrioritySet("Manual", 0);

			itemTextureResolver.LoadResourcepack(Resources.FurfSky_Reborn_1_5_0, "FurfSky");
			bazaarView = new();
			auctionHouseView = new();
			if (jsonStruct.Online) betterAHView = new();
			BazaarViewCommand = new RelayCommand((o) => { CurrentView = bazaarView; });
			AuctionHouseViewCommand = new RelayCommand((o) => { CurrentView = auctionHouseView; });
			BetterAHViewCommand = new RelayCommand((o) => { CurrentView = betterAHView; });
		}
	}
}

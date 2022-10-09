using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using YAHAC.Core;

namespace YAHAC.MVVM.ViewModel
{
	public class DebugDataViewModel : ObservableObject
	{
		private BackgroundTask backgroundTask;

		public DebugDataViewModel() : this(true)
		{
		}
		public DebugDataViewModel(bool KeepUpdated)
		{
			backgroundTask = new(TimeSpan.FromMilliseconds(100));
			if (KeepUpdated) StartBackgroundTask();
		}

		public void StartBackgroundTask()
		{
			backgroundTask.Start(CalculateProperties);
		}

		public async Task StopBackgroundTask()
		{
			await backgroundTask.StopAsync();
		}

		private void CalculateProperties()
		{
			//BazaarAge = (double)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - MainViewModel.bazaar.lastUpdated) / 1000;
			//AuctionHouseAge = ((double)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - MainViewModel.auctionHouse.lastUpdated) / 1000).ToString("N1");
			//HeaderRequestsInLastMinute = HypixelApiRequester.HeaderRequestsInLastMinute.ToString();
			//ApiRequestsInLastMinute = HypixelApiRequester.ApiRequestsInLastMinute.ToString();
			//UsedDataInMB = Math.Round((HypixelApiRequester.chc.PeroidDataTransfered / 1000000m), 1).ToString();
			OnPropertyChanged("BazaarAge");
			OnPropertyChanged("AuctionHouseAge");
			OnPropertyChanged("HeaderRequestsInLastMinute");
			OnPropertyChanged("ApiRequestsInLastMinute");
			OnPropertyChanged("UsedDataInMB");
		}

		//Properties
		public float BazaarAge //Max Value 100
		{
			get { return (float)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - MainViewModel.bazaar.lastUpdated) / 1000; }
		}

		public float AuctionHouseAge //Max Value 1000
		{
			get { return (float)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - MainViewModel.auctionHouse.lastUpdated) / 1000; }
		}

		public int HeaderRequestsInLastMinute //Max Value 1000
		{
			get { return HypixelApiRequester.HeaderRequestsInLastMinute; }
		}

		public int ApiRequestsInLastMinute //Max Value 1000
		{
			get { return HypixelApiRequester.ApiRequestsInLastMinute; }
		}

		public double UsedDataInMB //Max Value 1000
		{
			//https://www.teamscs.com/2016/09/ditch-decimal-data-type-use-double-c-code/ Deciam is over 100x times slower than double
			//get { return Math.Round((HypixelApiRequester.chc.PeroidDataTransfered / 1000000m), 1) }
			get { return (double)HypixelApiRequester.chc.PeroidDataTransfered / 1000000; }
		}



	}
}

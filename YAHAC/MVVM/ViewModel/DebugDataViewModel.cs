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
		private Task TimerTask;
		private readonly PeriodicTimer timer = new(TimeSpan.FromMilliseconds(100));
		private readonly CancellationTokenSource _cts = new();

		long TimeStamp_BZ;
		long TimeStamp_AH;
		public DebugDataViewModel()
		{
			TimeStamp_BZ = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			TimeStamp_AH = DateTimeOffset.Now.ToUnixTimeMilliseconds() - 2300;
			StartTimer();
		}

		//No idea how tf it works...
		public void StartTimer()
		{
			TimerTask = CalculateAgeAsync();
		}

		public async Task StopAsyncTimer()
		{
			if (TimerTask is null) { return; }
			_cts.Cancel();
			await TimerTask;
			_cts.Dispose();
		}

		private async Task CalculateAgeAsync()
		{
			try
			{
				while (await timer.WaitForNextTickAsync(_cts.Token))
				{
					CalculateAge();
				}
			}
			catch (OperationCanceledException)
			{

			}
		}

		private void CalculateAge()
		{
			BazaarAge = ((double)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - TimeStamp_BZ) / 1000).ToString("N1");
			AuctionHouseAge = ((double)(DateTimeOffset.Now.ToUnixTimeMilliseconds() - TimeStamp_AH) / 1000).ToString("N1");
		}
		//Properties
		private string _BazaarAge;
		public string BazaarAge
		{
			get { return _BazaarAge == null || Convert.ToDouble(_BazaarAge) > 999 ? "NaN" : _BazaarAge; }
			set
			{
				_BazaarAge = value;
				OnPropertyChanged();
			}
		}

		private string _AuctionHouseAge;
		public string AuctionHouseAge
		{
			get { return _AuctionHouseAge == null || Convert.ToDouble(_AuctionHouseAge) > 999 ? "NaN" : _AuctionHouseAge; ; }
			set
			{
				_AuctionHouseAge = value;
				OnPropertyChanged();
			}
		}

		private string _HeaderRequestsInLastMinute;
		public string HeaderRequestsInLastMinute
		{
			get { return _HeaderRequestsInLastMinute == null || Convert.ToInt32(_HeaderRequestsInLastMinute) > 999 ? "NaN" : _HeaderRequestsInLastMinute; }
			set
			{
				_HeaderRequestsInLastMinute = value;
				OnPropertyChanged();
			}
		}

		private string _ApiRequestsInLastMinute;
		public string ApiRequestsInLastMinute
		{
			get { return _ApiRequestsInLastMinute == null || Convert.ToInt32(_ApiRequestsInLastMinute) > 999 ? "NaN" : _ApiRequestsInLastMinute; }
			set
			{
				_ApiRequestsInLastMinute = value;
				OnPropertyChanged();
			}
		}

		private string _UsedDataInMB;
		public string UsedDataInMB
		{
			get { return _UsedDataInMB == null || Convert.ToDouble(_UsedDataInMB) > 999 ? "NaN" : _UsedDataInMB; }
			set
			{
				_UsedDataInMB = value;
				OnPropertyChanged();
			}
		}



	}
}

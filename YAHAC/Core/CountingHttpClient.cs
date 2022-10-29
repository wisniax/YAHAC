using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace YAHAC.Core
{
	/// <summary>
	/// Class that is responsible for counting bytes from GET requests.
	/// </summary>
	public class CountingHttpClient
	{
		private HttpClient httpConnector;

		private struct DataPlusTime
		{
			public ulong dataInB;
			public DateTime timestamp;

			public DataPlusTime()
			{
				dataInB = 0;
				timestamp = DateTime.Now;
			}

			public DataPlusTime(ulong dataInB)
			{
				this.dataInB = dataInB;
				timestamp = DateTime.Now;
			}
		}

		/// <summary>
		/// Data peroid in seconds for PeroidDataTransfered
		/// </summary>
		public UInt32 DataPeroid { get; set; } = 60;

		readonly private ConcurrentQueue<DataPlusTime> downloadedDataHistory;

		private ulong _LifeTimeDataTransfered;
		/// <summary>
		/// Data transfered during lifetime of this object
		/// </summary>
		public ulong LifeTimeDataTransfered
		{
			get { return _LifeTimeDataTransfered; }
		}

		/// <summary>
		/// Data transfered during determined peroid of time<br/>
		/// see <see cref="DataPeroid"/>
		/// </summary>
		public ulong PeroidDataTransfered
		{
			get
			{
				PurgeOldData();
				ulong sum = 0;
				foreach (var item in downloadedDataHistory)
				{
					sum += item.dataInB;
				}
				return sum;
			}
		}

		public CountingHttpClient()
		{
			httpConnector = new();
			downloadedDataHistory = new();
			_LifeTimeDataTransfered = 0;
		}

		//historic data that is old must be deleted, it is obvious L O L
		//it obvious too when u remove one entry then oldest is NULL
		//that for loop was nuts wrong, wrote by While gang
		private void PurgeOldData()
		{
            while (downloadedDataHistory.TryPeek(out DataPlusTime entry))
            {
				if (Convert.ToInt32((DateTime.Now - entry.timestamp).TotalSeconds) <= DataPeroid)
					break;
				downloadedDataHistory.TryDequeue(out _);
			}
		}

		//Saving data transfer both to lifetime and history
		private void LogData(ulong dataInB)
		{
			PurgeOldData();
			_LifeTimeDataTransfered += dataInB;
			downloadedDataHistory.Enqueue(new DataPlusTime(dataInB));
		}

		/// <summary>
		/// Presenting brand new function: DOWNLOAD DATA ASYNC WHILE KNOWING HOW MUCH DATA WAS DOWNLOADED©<br/>
		/// Only now u can know how much u downloaded to prove boss that u download movies only in LQ from company network. how sweet
		/// </summary>
		/// <param name="httpMethod">Http method to use</param>
		/// <param name="url">URL to call</param>
		/// <returns>Task wich i dont like from now on</returns>
		public async Task<HttpResponseMessage> SendHttpRequest(HttpMethod httpMethod, string url)
		{
			var response = await httpConnector.SendAsync(new HttpRequestMessage(httpMethod, url));
			if (httpMethod == HttpMethod.Get) LogData((ulong)(response.Content.Headers.ContentLength));
			return response;
		}

		/// <summary>
		/// Presenting brand new function: DOWNLOAD DATA ASYNC WHILE NOT CARING HOW MUCH DATA WAS DOWNLOADED©<br/>
		/// Only now u can download data and tell boss that nothing was downladed, especially the movies in HQ. noice
		/// </summary>
		/// <param name="httpMethod">Http method to use</param>
		/// <param name="url">URL to call</param>
		/// <returns>Task wich i dont like from now on</returns>
		public async Task<HttpResponseMessage> SendHttpRequest_Uncounted(HttpMethod httpMethod, string url)
		{
			var response = await httpConnector.SendAsync(new HttpRequestMessage(httpMethod, url));
			return response;
		}

	}
}

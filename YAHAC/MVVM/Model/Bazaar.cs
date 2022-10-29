using ITR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YAHAC.Core;

namespace YAHAC.MVVM.Model
{
	public class BazaarObj : ObservableObject
	{
		public bool success { get; set; }
		private long _lastUpdated;

		public long lastUpdated
		{
			get { return _lastUpdated; }
			set
			{
				_lastUpdated = value;
				OnPropertyChanged();
			}
		}
		public Dictionary<string, BazaarItemDef> products { get; set; }
	}
	public class BazaarItemDef
	{
		public string product_id { get; set; }
		public List<BzOrders> sell_summary { get; set; }
		public List<BzOrders> buy_summary { get; set; }
		public Quick_status quick_status { get; set; }
	}
	public class BzOrders
	{
		public UInt32 amount { get; set; }
		public double pricePerUnit { get; set; }
		public UInt16 orders { get; set; }

	}
	public class Quick_status
	{
		public string productId { get; set; }
		public double sellPrice { get; set; }
		public UInt32 sellVolume { get; set; }
		public UInt32 sellMovingWeek { get; set; }
		public UInt16 sellOrders { get; set; }
		public double buyPrice { get; set; }
		public UInt32 buyVolume { get; set; }
		public UInt32 buyMovingWeek { get; set; }
		public UInt16 buyOrders { get; set; }
	}

	public class Bazaar : BazaarObj
	{
		BackgroundTask backgroundTask;
		HypixelApiRequester hypixelApiRequester;

		KeyValuePair<HttpResponseHeaders, HttpContentHeaders> latestHeaders;

		DateTimeOffset? Header_LastModified;
		TimeSpan? Header_TimeOffset;
		bool ShouldRefresh;

		public delegate void BazaarUpdatedHandler(Bazaar source);
		public event BazaarUpdatedHandler BazaarUpdatedEvent;

		public Bazaar() : this(true) { }
		public Bazaar(bool KeepUpdated)
		{
			hypixelApiRequester = new(HypixelApiRequester.DataSources.Bazaar);
			Header_LastModified = new();
			ShouldRefresh = true;
			success = false;
			lastUpdated = 0;
			products = new Dictionary<string, BazaarItemDef>();
			backgroundTask = new(TimeSpan.FromMilliseconds(100));
			if (KeepUpdated) backgroundTask.Start(Refresh);
			else Refresh();
		}

		/// <summary>
		/// Executed everytime refresh succeded
		/// </summary>
		private void OnDownloadedItem()
		{
			BazaarUpdatedEvent?.Invoke(this);
		}

		/// <summary>
		/// Returns bazaar item data for specified item ID;
		/// </summary>
		/// <param name="key">Hypixel Item ID of an item.</param>
		/// <returns>BazaarItemDef if id was matched, otherwise null</returns>
		public BazaarItemDef GetBazaarItemDataFromID(string key)
		{
			if (key == null) return null;
			if (products.TryGetValue(key, out BazaarItemDef item)) return item;
			return null;
		}

		/// <summary>
		/// Uses page head property to determine whether body has updated <br/>
		/// When implemented properly should be:<br/>
		/// - even 10s faster than old refresh method!!!<br/>
		/// - much more consistent<br/>
		/// - use less data :) <br/>
		/// </summary>
		public void Refresh()
		{
			if (!ShouldPerform_Refresh()) { return; }
			var BZResult = Task.Run(async () => await hypixelApiRequester.GetBodyAsync()).Result;
			var serializedBazaar = BZResult.Content.ReadAsStringAsync().Result;
			//Save lastUpdated timestamp for success evaluation
			long last_lastUpdated = lastUpdated;
			deserialize(serializedBazaar);

			if (!success || (last_lastUpdated + 1000 >= lastUpdated))
			{
				return;
			}

			latestHeaders = new(BZResult.Headers, BZResult.Content.Headers);
			Header_TimeOffset = DateTimeOffset.Now - latestHeaders.Key.Date;
			Header_LastModified = latestHeaders.Value.LastModified > Header_LastModified ? latestHeaders.Value.LastModified : Header_LastModified;
			OnDownloadedItem();
			ShouldRefresh = false;
		}

		private bool ShouldPerform_Refresh()
		{
			TimeSpan timeSpanRefresh = new TimeSpan(0, 0, 0, 19, 700);
			return ShouldPerform_Refresh(timeSpanRefresh);
		}
		private bool ShouldPerform_Refresh(TimeSpan timeSpanRefresh)
		{
			if (ShouldRefresh == true || latestHeaders.Value == null || latestHeaders.Key == null) return true;
			var timePassed = DateTimeOffset.Now - Header_TimeOffset - (latestHeaders.Key.Date - latestHeaders.Key.Age);
			if (timeSpanRefresh >= timePassed) return false;

			//var head = hypixelApiRequester.GetHeadAsync().Result;
			var head = Task.Run(async () => await hypixelApiRequester.GetHeadAsync()).Result;
			latestHeaders = new(head.Headers, head.Content.Headers);
			Header_TimeOffset = DateTimeOffset.Now - latestHeaders.Key.Date;
			var ch = DateTimeOffset.Now - Header_TimeOffset;
			var rozn = latestHeaders.Key.Date - ch;
			var val = (latestHeaders.Key.Date - Header_LastModified);

			if (latestHeaders.Value.LastModified != Header_LastModified)
			{
				Header_LastModified = latestHeaders.Value.LastModified > Header_LastModified ? latestHeaders.Value.LastModified : Header_LastModified;
				ShouldRefresh = true;
				return true;
			}
			//if (latestHeaders.Key.Age+1<)
			return false;
		}


		/// <summary>
		/// Uses deprecated method to wait for api refresh.
		/// </summary>
		[Obsolete]
		public void refresh()
		{
			if (!(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastUpdated > 20000)) return;
			var serializedBazaar = hypixelApiRequester.GetBodyAsync().Result.Content.ReadAsStringAsync().Result;
			deserialize(serializedBazaar);
		}


		void deserialize(string serialized)
		{
			var deserialized = JsonSerializer.Deserialize<BazaarObj>(serialized);
			//if (deserialized.products.ContainsKey("BAZAAR_COOKIE")) deserialized.products.Remove("BAZAAR_COOKIE");
			success = deserialized.success;
			lastUpdated = deserialized.lastUpdated;
			products = deserialized.products;
		}
	}
}

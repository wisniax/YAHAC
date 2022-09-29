using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using YAHAC.Core;
using System.Text.Json;

namespace YAHAC.MVVM.Model
{
	/// <summary>
	/// All possible data in single auction
	/// The unnecessary lines are commented for memory preservation
	/// </summary>
	public class Auction
	{
		public string uuid { get; set; }
		public string auctioneer { get; set; }
		public string profile_id { get; set; }
		//public List<string> coop { get; set; }
		public long start { get; set; }
		public long end { get; set; }
		public string item_name { get; set; }
		public string item_lore { get; set; }
		public string extra { get; set; }
		public string category { get; set; }
		public string tier { get; set; }
		public int starting_bid { get; set; }
		public string item_bytes { get; set; }
		public bool claimed { get; set; }
		//public List<object> claimed_bidders { get; set; }
		public int highest_bid_amount { get; set; }
		public long last_updated { get; set; }
		public bool bin { get; set; }
		//public List<Bid> bids { get; set; }
		public string item_uuid { get; set; }
	}

	/// <summary>
	/// All data of single bid on an item
	/// </summary>
	public class Bid
	{
		public string auction_id { get; set; }
		public string bidder { get; set; }
		public string profile_id { get; set; }
		public int amount { get; set; }
		public object timestamp { get; set; }
	}

	/// <summary>
	/// AuctionHouse 1st layer stuff
	/// </summary>
	public class AuctionHousePage
	{
		public bool success { get; set; }
		public int page { get; set; }
		public int totalPages { get; set; }
		public int totalAuctions { get; set; }
		public long lastUpdated { get; set; }
		public List<Auction> auctions { get; set; }
	}

	public class AuctionHouse
	{
		public bool success { get; private set; }
		public long lastUpdated { get; set; }
		public int totalPages { get; set; }
		public int totalAuctions { get; set; }
		public Dictionary<string, List<Auction>> auctions { get; set; }


		BackgroundTask backgroundTask;
		HypixelApiRequester AHPageRequester;
		HypixelApiRequester AHEndedRequester;

		KeyValuePair<HttpResponseHeaders, HttpContentHeaders> latestHeaders;

		DateTimeOffset? Header_LastModified;
		TimeSpan? Header_TimeOffset;
		bool ShouldRefresh;

		public delegate void AHUpdatedHandler(AuctionHouse source);
		public event AHUpdatedHandler AHUpdatedEvent;

		bool WholeAHGathered;

		public AuctionHouse() : this(true) { }
		public AuctionHouse(bool KeepUpdated)
		{
			success = false;
			WholeAHGathered = false;

			AHPageRequester = new(HypixelApiRequester.DataSources.AuctionHouse_Auctions);
			AHEndedRequester = new(HypixelApiRequester.DataSources.AuctionHouse_Ended);

			Header_LastModified = new();
			ShouldRefresh = true;
			lastUpdated = 0;

			//Define all left class variables
			totalPages = new();
			totalAuctions = new();
			auctions = new();

			backgroundTask = new(TimeSpan.FromMilliseconds(100));
			if (KeepUpdated) backgroundTask.Start(Refresh);
			else Refresh();
		}

		/// <summary>
		/// Executed everytime refresh succeded
		/// </summary>
		private void OnDownloadedItem()
		{
			AHUpdatedEvent?.Invoke(this);
		}

		/// <summary>
		/// Returns Auction House auctions list for specified item ID;
		/// </summary>
		/// <param name="key">Hypixel Item ID of an item</param>
		/// <returns>List of Auction if id was matched, otherwise null </returns>
		public List<Auction> GetBazaarItemDataFromID(string key)
		{
			if (key == null) return null;
			if (auctions.TryGetValue(key, out List<Auction> items)) return items;
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
			var AHPageResult = Task.Run(async () => await AHPageRequester.GetBodyAsync()).Result;
			var serializedPage = AHPageResult.Content.ReadAsStringAsync().Result;
			long last_lastUpdated = lastUpdated;            //Save lastUpdated for success evaluation

			//Deserialization goes hereee JsonSerializer.Deserialize<AuctionHousePage>(serialized)
			var deserializedPage = JsonSerializer.Deserialize<AuctionHousePage>(serializedPage);
			if (!AddPageToAuctions(deserializedPage)) return;

			//Whole ah download
			//if (!WholeAHGathered)
			//{
			//	for (int i = 0; i < deserializedPage.totalPages; i++)
			//	{
			//		throw new NotImplementedException();
			//	}
			//}

			if (!success || (last_lastUpdated + 1000 >= lastUpdated)) return;

			latestHeaders = new(AHPageResult.Headers, AHPageResult.Content.Headers);
			Header_TimeOffset = DateTimeOffset.Now - latestHeaders.Key.Date;
			Header_LastModified = latestHeaders.Value.LastModified > Header_LastModified ? latestHeaders.Value.LastModified : Header_LastModified;
			OnDownloadedItem();
			ShouldRefresh = false;
		}

		//Some Copy-Paste from Bazaar ... dont mind me lmao
		private bool ShouldPerform_Refresh()
		{
			TimeSpan timeSpanRefresh = new TimeSpan(0, 0, 0, 59, 700);
			return ShouldPerform_Refresh(timeSpanRefresh);
		}
		private bool ShouldPerform_Refresh(TimeSpan timeSpanRefresh)
		{
			if (ShouldRefresh == true || latestHeaders.Value == null || latestHeaders.Key == null) return true;
			var timePassed = DateTimeOffset.Now - Header_TimeOffset - (latestHeaders.Key.Date - latestHeaders.Key.Age);
			if (timeSpanRefresh >= timePassed) return false;

			//var head = hypixelApiRequester.GetHeadAsync().Result;
			var head = Task.Run(async () => await AHPageRequester.GetHeadAsync()).Result;
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

		bool AddPageToAuctions(AuctionHousePage page)
		{
			if (page == null) return false;
			if (!page.success) return false;

			foreach (var item in page.auctions)
			{
				var cus = NBTReader.ReadNBTFromB64String(item.item_bytes);
			}

			return true;
		}

	}
}

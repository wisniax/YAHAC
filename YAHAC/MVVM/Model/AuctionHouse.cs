using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using YAHAC.Core;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Windows.Controls;

namespace YAHAC.MVVM.Model
{

	public class EndedAuction
	{
		/// <summary>
		/// Auction's unique ID. <br/>
		/// Smh it cant be named the uuid bc Hypixel
		/// </summary>
		public string auction_id { get; set; }
		public string seller { get; set; }
		public string seller_profile { get; set; }
		public string buyer { get; set; }
		public object timestamp { get; set; }
		public int price { get; set; }
		public bool bin { get; set; }
		public string item_bytes { get; set; }
		public string HyPixel_ID { get; set; } //Assigned right after download
	}

	public class AuctionHouseEndedPage
	{
		public bool success { get; set; }
		public long lastUpdated { get; set; }
		public List<EndedAuction> auctions { get; set; }
	}

	/// <summary>
	/// All possible data in single auction
	/// The unnecessary lines are commented for memory preservation
	/// </summary>
	public class Auction
	{
		/// <summary>
		/// Auction's unique ID
		/// </summary>
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
		public Int64 starting_bid { get; set; }
		public string item_bytes { get; set; }
		public bool claimed { get; set; }
		//public List<object> claimed_bidders { get; set; }
		public Int64 highest_bid_amount { get; set; }
		public long last_updated { get; set; }
		public bool bin { get; set; }
		//public List<Bid> bids { get; set; }
		/// <summary>
		/// Item's unique ID ( Rare items have unique ID to check whether they are duped :) )
		/// </summary>
		public string item_uuid { get; set; }
		public string HyPixel_ID { get; set; } //Assigned right after download
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
		/// <summary>
		/// Did page fetch successfully
		/// </summary>
		public bool success { get; set; }
		/// <summary>
		/// Page number
		/// </summary>
		public int page { get; set; }
		/// <summary>
		/// Amount of AH pages at fetch time
		/// </summary>
		public int totalPages { get; set; }
		public int totalAuctions { get; set; }
		/// <summary>
		/// Unix timestamp
		/// </summary>
		public long lastUpdated { get; set; }
		/// <summary>
		/// Auctions list
		/// </summary>
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

		public AuctionHouse() : this(true, true) { }
		public AuctionHouse(bool KeepUpdated, bool GatherWholeAH)
		{
			success = false;
			WholeAHGathered = !GatherWholeAH;

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

			if (!WholeAHGathered) GatherWholeAH(deserializedPage.totalPages);

			AddPageToAuctions(deserializedPage, last_lastUpdated);

			var AHEndedResult = Task.Run(async () => await AHEndedRequester.GetBodyAsync()).Result;
			var serializedEndedPage = AHEndedResult.Content.ReadAsStringAsync().Result;
			var deserializedEndedPage = JsonSerializer.Deserialize<AuctionHouseEndedPage>(serializedEndedPage);
			RemoveEndedAuctions(deserializedEndedPage);

			lastUpdated = deserializedPage.lastUpdated;
			success = deserializedPage.success;
			totalAuctions = deserializedPage.totalAuctions;
			totalPages = deserializedPage.totalPages;

			//if (!success || (last_lastUpdated + 1000 >= lastUpdated)) return;

			latestHeaders = new(AHPageResult.Headers, AHPageResult.Content.Headers);
			Header_TimeOffset = DateTimeOffset.Now - latestHeaders.Key.Date;
			Header_LastModified = latestHeaders.Value.LastModified > Header_LastModified ? latestHeaders.Value.LastModified : Header_LastModified;
			ShouldRefresh = false;
			OnDownloadedItem();
			int totalItems = 0;
			foreach (var value in auctions.Values)
			{
				totalItems += value.Count;
			}
			totalItems += 0;
		}

		private void GatherWholeAH(int totalPages)
		{
			auctions.Clear();
			List<Task> tasks = new();
			for (int i = 1; i < totalPages; i++)
			{
				int other_i = i;
				tasks.Add(Task.Run(() =>
				{
					var result = Task.Run(async () => await AHPageRequester.GetBodyAsync(other_i)).Result;
					var serialized = result.Content.ReadAsStringAsync().Result;
					var deserialized = JsonSerializer.Deserialize<AuctionHousePage>(serialized);
					AddPageToAuctions(deserialized);
				}));
			}
			Task.WaitAll(tasks.ToArray());
			WholeAHGathered = true;
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

		void AddPageToAuctions(AuctionHousePage page, long? lastUpdated = null)
		{
			if (page == null) throw new ApplicationException();
			if (!page.success) throw new ApplicationException();

			//I dont care bout regular auctions for now
			page.auctions.RemoveAll((a) => !a.bin);

			var remainingAuctions = new List<Auction>();

			if (lastUpdated != null)
			{
				remainingAuctions = page.auctions.FindAll((a) => a.last_updated < lastUpdated);
				page.auctions.RemoveAll((a) => a.last_updated < lastUpdated);
			}

			NBTReader nbtReader = new();
			foreach (var item in page.auctions)
			{
				item.HyPixel_ID = nbtReader.GetIdFromB64String(item.item_bytes);
			}

			//Move to dictionary

			foreach (var item in page.auctions)
			{
				if (auctions.TryGetValue(item.HyPixel_ID, out var existingItems) == true)
				{
					existingItems.Add(item);
				}
				else
				{
					var cus = auctions.TryAdd(item.HyPixel_ID, new List<Auction> { item });
					if (!cus) { throw new Exception(); }
				}
			}
			page.auctions = remainingAuctions;
		}

		void RemoveEndedAuctions(AuctionHouseEndedPage endedAuctions)
		{
			int removed = new();
			NBTReader nbtReader = new();
			endedAuctions.auctions.RemoveAll((a) => !a.bin);

			foreach (var item in endedAuctions.auctions)
			{
				item.HyPixel_ID = nbtReader.GetIdFromB64String(item.item_bytes);
			}

			foreach (var item in endedAuctions.auctions)
			{
				auctions.TryGetValue(item.HyPixel_ID, out var lista);
				if (lista == null) { continue; }
				removed += lista.RemoveAll((a) => a.uuid == item.auction_id);
			}
		}
	}
}

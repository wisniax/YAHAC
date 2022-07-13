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
	public class Bazaar : DataPatterns.BazaarObj
	{
		BackgroundTask backgroundTask;
		HypixelApiRequester hypixelApiRequester;
		KeyValuePair<HttpResponseHeaders, HttpContentHeaders> latestHeaders;
		DateTimeOffset? Header_LastModified;
		TimeSpan? Header_TimeOffset;
		bool ShouldRefresh;

		public Bazaar() : this(true) { }
		public Bazaar(bool KeepUpdated)
		{
			hypixelApiRequester = new(HypixelApiRequester.DataSources.Bazaar);
			Header_LastModified = new();
			ShouldRefresh = true;
			success = false;
			lastUpdated = 0;
			products = new Dictionary<string, DataPatterns.BazaarItemDef>();
			backgroundTask = new(TimeSpan.FromMilliseconds(100));
			if (KeepUpdated) backgroundTask.Start(Refresh);
			else Refresh();
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
			var BZResult = hypixelApiRequester.GetBodyAsync().Result;
			var serializedBazaar = BZResult.Content.ReadAsStringAsync().Result;
			var last_lastUpdated = lastUpdated;
			deserialize(serializedBazaar);
			if (!success || last_lastUpdated >= lastUpdated) return;

			latestHeaders = new(BZResult.Headers, BZResult.Content.Headers);
			Header_TimeOffset = DateTimeOffset.Now - latestHeaders.Key.Date;
			Header_LastModified = latestHeaders.Value.LastModified > Header_LastModified ? latestHeaders.Value.LastModified : Header_LastModified;

			ShouldRefresh = false;
		}

		private bool ShouldPerform_Refresh()
		{
			if (ShouldRefresh == true || latestHeaders.Value == null || latestHeaders.Key == null) return true;

			//if ()

			var head = hypixelApiRequester.GetHeadAsync().Result;
			latestHeaders = new(head.Headers, head.Content.Headers);
			Header_TimeOffset = DateTimeOffset.Now - latestHeaders.Key.Date;
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

		void updateTelemetry()
		{

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
			var deserialized = JsonSerializer.Deserialize<DataPatterns.BazaarObj>(serialized);
			success = deserialized.success;
			lastUpdated = deserialized.lastUpdated;
			products = deserialized.products;
		}
	}
}

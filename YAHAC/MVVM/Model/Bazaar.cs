using System;
using System.Collections.Generic;
using System.Linq;
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

		public Bazaar() : this(true)
		{
		}
		public Bazaar(bool KeepUpdated)
		{
			hypixelApiRequester = new(HypixelApiRequester.DataSources.Bazaar);
			success = false;
			lastUpdated = 0;
			products = new Dictionary<string, DataPatterns.BazaarItemDef>();
			backgroundTask = new(TimeSpan.FromMilliseconds(250));
			if (KeepUpdated) backgroundTask.Start(refresh);
			else refresh();
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

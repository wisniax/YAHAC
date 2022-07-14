using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YAHAC.Core;

namespace YAHAC.MVVM.Model
{
	public class ItemsRepository : DataPatterns.ItemRepo
	{
		public ItemsRepository()
		{
			using (HypixelApiRequester hypixelApiRequester = new(HypixelApiRequester.DataSources.Items))
			{
				while (!success)
				{
					deserialize(hypixelApiRequester.GetBodyAsync().Result.Content.ReadAsStringAsync().Result);
				}
			}
		}

		public string ID_to_NAME(string id)
		{
			return ID_to_ITEM(id)?.name;
		}

		public DataPatterns.Item ID_to_ITEM(string id)
		{
			return items.Find(x => (x.id == id));
		}

		void deserialize(string serialized)
		{
			var deserialized = JsonSerializer.Deserialize<DataPatterns.ItemRepo>(serialized);
			success = deserialized.success;
			lastUpdated = deserialized.lastUpdated;
			items = deserialized.items;
		}
	}
}

using ITR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using YAHAC.MVVM.Model;
using YAHAC.Properties;

namespace YAHAC.Converters
{
	internal class ItemToItemName : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return "Not Found";
			string str = "";
			Rarity tier = 0;
			var name = "";
			Item item;
			if (value is Item)
			{
				item = value as Item;
				tier = item.Tier;
				name = item.Name;
			}
			else if (value is Auction)
			{
				var val = value as Auction;
				tier = Item.GetRarityFromString(val.tier);
				name = val.item_name;
			}
			else if (value is ItemsToSearchForCatalogue)
			{
				var val = value as ItemsToSearchForCatalogue;
				tier = Rarity.Custom;
				name = val.Name;
			}
			else throw new NotImplementedException();

			switch (tier)
			{
				case Rarity.Common:
					str = "§f";
					break;
				case Rarity.Uncommon:
					str = "§a";
					break;
				case Rarity.Rare:
					str = "§9";
					break;
				case Rarity.Epic:
					str = "§5";
					break;
				case Rarity.Legendary:
					str = "§6";
					break;
				case Rarity.Mythic:
					str = "§d";
					break;
				case Rarity.Divine:
					str = "§b";
					break;
				case Rarity.Special:
					str = "§c";
					break;
				case Rarity.Very_Special:
					str = "§c";
					break;
				case Rarity.Custom:
					str = "§e";
					break;
			}
			return str += name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

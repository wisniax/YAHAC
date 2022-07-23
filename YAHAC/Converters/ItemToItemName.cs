using ITR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YAHAC.Converters
{
	internal class ItemToItemName : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "";
			string str = "";
			Item item;
			item = value as Item;
			switch (item.Tier)
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
			return str += item.Name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

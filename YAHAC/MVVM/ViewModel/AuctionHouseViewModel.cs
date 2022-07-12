using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAHAC.MVVM.ViewModel
{
	internal class AuctionHouseViewModel
	{
		public List<string> strings { get; set; }
		public AuctionHouseViewModel()
		{
			strings = new List<string>();
			for (int i = 0; i < 3000000; i++)
			{
				strings.Add("JD2137"+i.ToString());
			}
		}
	}
}

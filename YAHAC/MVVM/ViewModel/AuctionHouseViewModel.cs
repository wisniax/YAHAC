using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAHAC.MVVM.ViewModel
{
	internal class AuctionHouseViewModel
	{
		public AuctionHouseViewModel()
		{
			List<Task> tasks = new List<Task>();
			for (int j = 0; j < 5; j++)
			{
				var stringsT = new List<string>();
				tasks.Add(Task.Factory.StartNew(() =>
				{
					for (int i = 0; i < 600000; i++)
					{
						stringsT.Add("JD2137" + i.ToString());
					}
				}));
			}
			Task.WaitAll(tasks.ToArray());
		}
	}
}

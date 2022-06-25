using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAHAC.MVVM.Model;
using YAHAC.MVVM.View;

namespace YAHAC.MVVM.ViewModel
{
	internal class BazaarViewModel
	{
		public BazaarView ViewModel { get; set; }

		public BazaarViewModel()
		{
			ViewModel = new BazaarView();
			ObservableCollection<object> items = new ObservableCollection<object>();
			ViewModel.ItemsList.ItemsSource = items;
			for (int i = 0; i < 400; i++)
			{
				items.Add(new MinecraftItemBox());
			}
		}
		~BazaarViewModel()
		{
			ViewModel = null;
		}

	}
}

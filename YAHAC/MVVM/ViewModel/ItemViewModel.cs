using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAHAC.Properties;
using YAHAC.Core;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace YAHAC.MVVM.ViewModel
{
	public class ItemViewModel : ObservableObject
	{
		private Bitmap _Texture;

		public Bitmap Texture
		{
			get { return _Texture; }
			set
			{
				_Texture = value;
				OnPropertyChanged();
			}
		}

		public ItemViewModel()
		{
			Texture = new Bitmap(Properties.Resources.NoTextureMark);
		}
	}
}

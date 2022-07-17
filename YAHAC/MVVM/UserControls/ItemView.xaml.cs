using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ITR;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for ItemView.xaml
	/// </summary>
	public partial class ItemView : UserControl
	{

		public Item item
		{
			get { return (Item)GetValue(itemProperty); }
			set { SetValue(itemProperty, value); }
		}

		// Using a DependencyProperty as the backing store for item.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty itemProperty =
			DependencyProperty.Register("item", typeof(Item), typeof(ItemView), new PropertyMetadata(
				new Item(null, null, Material.AIR, true, Properties.Resources.NoTextureMark, false)));

		public ItemView()
		{
			InitializeComponent();
		}
	}
}

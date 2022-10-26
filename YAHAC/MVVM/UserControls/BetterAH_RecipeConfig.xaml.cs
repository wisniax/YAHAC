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
using YAHAC.Properties;

namespace YAHAC.MVVM.UserControls
{
	/// <summary>
	/// Interaction logic for BetterAH_RecipeConfig.xaml
	/// </summary>
	public partial class BetterAH_RecipeConfig : UserControl
	{

		public ItemToSearchFor itemToSearchFor
		{
			get { return (ItemToSearchFor)GetValue(itemToSearchForProperty); }
			set { SetValue(itemToSearchForProperty, value); }
		}

		// Using a DependencyProperty as the backing store for itemToSearchFor.  This enables animation, styling, binding, etc...
		//https://stackoverflow.com/questions/25989018/wpf-usercontrol-twoway-binding-dependency-property
		public static readonly DependencyProperty itemToSearchForProperty =
			DependencyProperty.Register("itemToSearchFor", typeof(ItemToSearchFor), typeof(BetterAH_RecipeConfig), new FrameworkPropertyMetadata(
			null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


		//Someday https://www.codeproject.com/Articles/44920/A-Reusable-WPF-Autocomplete-TextBox or https://www.codeproject.com/Articles/31947/WPF-AutoComplete-Folder-TextBox
		public BetterAH_RecipeConfig()
		{
			InitializeComponent();
		}
	}
}

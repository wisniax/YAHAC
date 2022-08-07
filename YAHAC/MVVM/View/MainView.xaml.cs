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
using System.Windows.Shapes;
using YAHAC.MVVM.ViewModel;
using YAHAC.Core;
using YAHAC.Properties;
using System.Diagnostics;

namespace YAHAC.MVVM.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            var dataCont = ((MainViewModel)this.DataContext);
            var Starting_Ui = MainViewModel.settings.Default.Starting_Ui;
            if (Starting_Ui.Equals(UserInterfaces.Bazaar)) { BtnBZ.IsChecked = true; dataCont.BazaarViewCommand.Execute(dataCont); }
            if (Starting_Ui.Equals(UserInterfaces.AuctionHouse)) { BtnAH.IsChecked = true; dataCont.AuctionHouseViewCommand.Execute(dataCont); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MainViewModel.settings.Default.MinecraftItemBox_Size = 34;
            MainViewModel.settings.Default.DebugVisibility = Visibility.Hidden;
            MainViewModel.settings_Changed();
        }
    }
}

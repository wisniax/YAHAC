using ITR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YAHAC.Core;
using YAHAC.MVVM.Model;
using YAHAC.MVVM.UserControls;

namespace YAHAC.MVVM.ViewModel
{
	internal class BetterAHViewModel : ObservableObject
	{
		SoundPlayer soundPlayer;

		private ObservableCollection<object> _Items;
		/// <summary>
		/// Collection of auctions in matchingItems list presented in graphical format
		/// </summary>
		public ObservableCollection<object> Items
		{
			get { return _Items; }
			set
			{
				_Items = value;
				OnPropertyChanged();
			}
		}
		private bool _AdditonalInfo_Visible;
		public bool AdditionalInfo_Visible
		{
			get { return _AdditonalInfo_Visible; }
			set
			{
				_AdditonalInfo_Visible = value;
				OnPropertyChanged();
			}
		}
		private ItemView _SelectedItemView;
		public ItemView SelectedItemView
		{
			get { return _SelectedItemView; }
			set
			{
				if (value == null) { AdditionalInfo_Visible = false; return; }
				_SelectedItemView = value;
				OnPropertyChanged();
				AdditionalInfo_Visible = true;
			}
		}
		private Point _CanvasPoint;
		public Point CanvasPoint
		{
			get { return _CanvasPoint; }
			set
			{
				_CanvasPoint = value;
				OnPropertyChanged();
			}
		}

		string highlitedAuction_uuid;

		//public List<GuiCode.itemUC> itemsUi;

		public BetterAHViewModel()
		{
			Items = new();
			highlitedAuction_uuid = "";
			MainViewModel.betterAH.BetterAHUpdatedEvent += BetterAH_Updated;
			if (Items.Count == 0 && MainViewModel.betterAH.success) LoadBetterAH();
			soundPlayer = new(Properties.Resources.notify_sound);
			//if (MainViewModel.settings.Default.PlaySound)
			//{
			//	soundPlayer = new(Properties.Resources.notify_sound);
			//}
		}

		void LoadBetterAH()
		{
			Items = new();
			if (!MainViewModel.betterAH.success) return;
			foreach (var auction in MainViewModel.betterAH.MatchingItems)
			{
				if (auction == null) continue;

				var item = MainViewModel.itemTextureResolver.GetItemFromID(auction.HyPixel_ID);
				//if (item == null) continue;
				// In case item does not exist in Hypixel API create an unknown one with id as its name
				if (item == null)
				{
					Converters.BitmapToMemoryStream convbtm = new Converters.BitmapToMemoryStream();
					item = new Item(
						  auction.HyPixel_ID,
						  auction.HyPixel_ID,
						  Material.AIR,
						  true,
						  convbtm.Convert(Properties.Resources.NoTextureMark, null, null, CultureInfo.CurrentCulture) as MemoryStream);
				}
				ItemView itemBox = new(item, auction);
				Items?.Add(itemBox);
			}
		}

		private void BetterAH_Updated(Model.BetterAH source)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					if (source == null) return;
					if (!source.success) return;
					LoadBetterAH();
					var auc = MainViewModel.betterAH.GetHighestPriorityAuction();
					if (auc != null)
					{
						var uuid = auc.uuid;
						playSound(uuid);
						JadeRald(uuid);
						highlitedAuction_uuid = uuid;
					}
					return;
				});
			}
			else
			{
				if (source == null) return;
				if (!source.success) return;
				LoadBetterAH();
				var auc = MainViewModel.betterAH.GetHighestPriorityAuction();
				if (auc != null)
				{
					var uuid = auc.uuid;
					playSound(uuid);
					JadeRald(uuid);
					highlitedAuction_uuid = uuid;
				}
				return;
			}
		}

		void playSound(string uuid)
		{
			if (highlitedAuction_uuid == uuid) { return; }
			var lista = MainViewModel.betterAH.ItemsToSearchFor.FindAll((a) => a.priority >= 1);
			foreach (var item in lista)
			{
				if (MainViewModel.betterAH.MatchingItems.Exists((a) => a.HyPixel_ID == item.item_dictKey))
				{
					soundPlayer.Play();
					return;
				}
			}
		}

		void JadeRald(string uuid)
		{
			if (highlitedAuction_uuid == uuid) { return; }
			var lista = MainViewModel.betterAH.ItemsToSearchFor.FindAll((a) => a.priority >= 1);
			lista.Sort((a, b) => b.priority.CompareTo(a.priority));
			foreach (var item in lista)
			{
				if (MainViewModel.betterAH.MatchingItems.Exists((a) => a.HyPixel_ID == item.item_dictKey))
				{
					var smth = MainViewModel.betterAH.MatchingItems.Find((a) => a.HyPixel_ID == item.item_dictKey);
					CopyToClipboard("/viewauction " + smth.uuid);
					return;
				}
			}
		}

		void CopyToClipboard(string str)
		{
			var thread = new Thread(() => Clipboard.SetText(str));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
		}

		public void MouseDoubleClicked(object sender, MouseButtonEventArgs e)
		{
			if (SelectedItemView.Tag == null) return;
			CopyToClipboard("/viewauction " + (SelectedItemView.Tag as Auction).uuid);
		}
		//This either
		//private void RenderItemName(itemUC sender, GuiCode.itemUC.MouseEvents mouseEvents)
		//{
		//	switch (mouseEvents)
		//	{
		//		case GuiCode.itemUC.MouseEvents.Enter:
		//			labelItemNameTip.Enabled = true;
		//			labelItemNameTip.Text = Properties.AllItemsREPO.IDtoNAME(sender.item_id);
		//			labelItemNameTip.Location = CalcPointPosition(labelItemNameTip);
		//			labelItemNameTip.Refresh();
		//			labelItemNameTip.Visible = true;
		//			break;
		//		case GuiCode.itemUC.MouseEvents.LocationChanged:
		//			labelItemNameTip.Location = CalcPointPosition(labelItemNameTip);
		//			break;
		//		case GuiCode.itemUC.MouseEvents.Click:
		//			labelItemNameTip.Visible = false;
		//			labelItemNameTip.Enabled = false;
		//			CopyToClipboard("/viewauction " + ((AuctionHouseFetcher.itemData)sender.Tag).uuid);
		//			break;
		//		case GuiCode.itemUC.MouseEvents.Leave:
		//			labelItemNameTip.Visible = false;
		//			labelItemNameTip.Enabled = false;
		//			break;
		//		default:
		//			break;
		//	}
		//}
	}
}

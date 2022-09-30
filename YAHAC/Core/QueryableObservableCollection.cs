using ITR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YAHAC.Core
{
	//Someday code's gonna be unified including this... but firstly lets just make it work...

	//public class QueryableObservableCollection : ObservableObject
	//{
	//	private ObservableCollection<object> _Items;
	//	public ObservableCollection<object> Items
	//	{
	//		get { return _Items; }
	//		set
	//		{
	//			_Items = value;
	//			OnPropertyChanged();
	//		}
	//	}

	//	private ObservableCollection<object> HiddenItems;

	//	private bool _AdditonalInfo_Visible;
	//	public bool AdditionalInfo_Visible
	//	{
	//		get { return _AdditonalInfo_Visible; }
	//		set
	//		{
	//			_AdditonalInfo_Visible = value;
	//			OnPropertyChanged();
	//		}
	//	}

	//	private Item _SelectedItem;
	//	public Item SelectedItem
	//	{
	//		get { return _SelectedItem; }
	//		set
	//		{
	//			if (value == null) { AdditionalInfo_Visible = false; return; }
	//			_SelectedItem = value;
	//			OnPropertyChanged();
	//			AdditionalInfo_Visible = true;
	//		}
	//	}

	//	private Point _CanvasPoint;
	//	public Point CanvasPoint
	//	{
	//		get { return _CanvasPoint; }
	//		set
	//		{
	//			_CanvasPoint = value;
	//			OnPropertyChanged();
	//		}
	//	}

	//	private string _SearchQuery;
	//	public string SearchQuery
	//	{
	//		get { return _SearchQuery; }
	//		set
	//		{
	//			_SearchQuery = value;
	//			OnPropertyChanged();
	//			OnSearchQueryChanged();
	//		}
	//	}
	//}
}

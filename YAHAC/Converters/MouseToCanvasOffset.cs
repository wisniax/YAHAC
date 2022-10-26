using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace YAHAC.Converters
{
	public class MouseToCanvasOffset : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return null;
			if (targetType.BaseType.Name != "UserControl") throw new NotImplementedException();
			var control = value as UserControl;
			Panel canvas = null;
			foreach (var item in (control.Content as Panel).Children)
			{
				if (item.GetType().IsEquivalentTo(typeof(Canvas))) canvas = item as Panel;
			}
			Size CanvasSize = new((canvas.Children[0] as FrameworkElement).ActualWidth, (canvas.Children[0] as FrameworkElement).ActualHeight);
			Size ControlSize = new(control.ActualWidth, control.ActualHeight);
			Point mousePosition = Mouse.GetPosition(control);
			Point point = new(mousePosition.X, mousePosition.Y);
			point.Offset(15, -30);
			if (point.X + CanvasSize.Width > ControlSize.Width) point.X = point.X - 2 * (point.X - mousePosition.X)-CanvasSize.Width;
			if (mousePosition.Y + CanvasSize.Height - (mousePosition.Y - point.Y) > ControlSize.Height) point.Y = ControlSize.Height - CanvasSize.Height;
			if (point.Y < 0&&point.Y+ ControlSize.Height>=CanvasSize.Height) point.Y = 0;
			return point;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

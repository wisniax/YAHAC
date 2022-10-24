using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace YAHAC.Converters
{
	public class NumberToString : IValueConverter
	{
		/// <summary>
		/// Converts most number types to string
		/// </summary>
		/// <param name="value">number to convert</param>
		/// <param name="targetType">Not used. Defaults to string</param>
		/// <param name="parameter">Specifies max value. When exceeded returns NaN</param>
		/// <param name="culture"></param>
		/// <returns>Text representation (string) of given number</returns>
		/// <exception cref="NotImplementedException">This Type was not implemented</exception>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//Hie Hie https://stackoverflow.com/questions/15777745/how-does-comparison-operator-works-with-null-int
			int? maxValue = parameter == null ? null : System.Convert.ToInt32(parameter);
			switch (value)
			{
				case null:
					return "NaN";
				case Double d:
					if (d >= maxValue) return "NaN";
					return d.ToString("N1", culture);
				case UInt32 d:
					if (d >= maxValue) return "NaN";
					return d.ToString("N0", culture);
				case UInt16 d:
					if (d >= maxValue) return "NaN";
					return d.ToString("N0", culture);
				case float d:
					if (d >= maxValue) return "NaN";
					return d.ToString("N1", culture);
				case int d:
					if (d >= maxValue) return "NaN";
					return d.ToString("N0", culture);
				case long d:
					if (d >= maxValue) return "NaN";
					return d.ToString("N0", culture);
				default:
					throw new NotImplementedException();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

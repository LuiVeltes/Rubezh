﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Controls.Converters
{
	public class IfConverter : IValueConverter
	{
		public object TrueValue { get; set; }
		public object FalseValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool bValue = System.Convert.ToBoolean(value);
			return bValue ? TrueValue : FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

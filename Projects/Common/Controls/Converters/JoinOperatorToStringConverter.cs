﻿using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Automation;

namespace Controls.Converters
{
	public class JoinOperatorToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((JoinOperator)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
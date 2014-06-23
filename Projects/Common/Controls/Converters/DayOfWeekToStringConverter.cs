﻿using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Automation;

namespace Controls.Converters
{
	public class DayOfWeekToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is DayOfWeekType))
				return "";
			return ((DayOfWeekType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (DayOfWeekType)value;
		}
	}
}
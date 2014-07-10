﻿using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.GK;

namespace GKModule.Converters
{
	public class JournalItemTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((XJournalItemType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}
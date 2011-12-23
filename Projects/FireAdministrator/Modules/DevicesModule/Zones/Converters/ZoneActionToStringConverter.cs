﻿using System;
using System.Windows.Data;
using FiresecAPI.Models;
using Common;

namespace DevicesModule.Converters
{
    public class ZoneActionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ZoneActionType)
            {
                return EnumHelper.GetEnumDescription((ZoneActionType)value);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
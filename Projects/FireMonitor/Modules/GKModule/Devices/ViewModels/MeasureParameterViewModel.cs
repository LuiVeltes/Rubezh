﻿using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class MeasureParameterViewModel : BaseViewModel
	{
		public GKDevice Device { get; set; }
		public string Name { get; set; }
		public bool IsDelay { get; set; }
		public GKMeasureParameter DriverParameter { get; set; }

		double _value;
		public double Value
		{
			get { return _value; }
			set
			{
				_value = value;
				OnPropertyChanged(() => Value);
			}
		}

		string _stringValue;
		public string StringValue
		{
			get { return _stringValue; }
			set
			{
				_stringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}
	}
}
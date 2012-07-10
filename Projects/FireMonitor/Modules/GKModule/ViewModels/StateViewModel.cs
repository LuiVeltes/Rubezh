﻿using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public string DeviceName { get; set; }
		public DriverState DriverState { get; set; }
	}
}
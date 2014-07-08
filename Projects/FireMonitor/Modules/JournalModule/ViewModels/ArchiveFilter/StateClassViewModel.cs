﻿using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;

namespace JournalModule.ViewModels
{
	public class StateClassViewModel : CheckBoxItemViewModel
	{
		public StateClassViewModel(XStateClass stateClass)
		{
			StateClass = stateClass;
			Name = stateClass.ToDescription();
		}

		public XStateClass StateClass { get; private set; }
		public string Name { get; private set; }
	}
}
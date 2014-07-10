﻿using FiresecAPI.Journal;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace FiltersModule.ViewModels
{
	public class FilterViewModel : BaseViewModel
	{
		public JournalFilter Filter { get; set; }

		public FilterViewModel(JournalFilter filter)
		{
			Filter = filter;
		}

		public string Name
		{
			get { return Filter.Name; }
			set
			{
				Filter.Name = value;
				OnPropertyChanged("Name");
				ServiceFactory.SaveService.FilterChanged = true;
			}
		}

		public void Update(JournalFilter filter)
		{
			Filter = filter;
			OnPropertyChanged("Name");
		}
	}
}
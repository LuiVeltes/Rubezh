﻿using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class OrganisationViewModel : BaseViewModel
	{
		public Organization Organisation { get; private set; }

		public OrganisationViewModel(Organization organisation)
		{
			Organisation = organisation;
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}
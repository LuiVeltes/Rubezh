﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using FiresecClient;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class ShedulesViewModel : ViewPartViewModel
	{
		public ShedulesViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter());
			var employeeShedules = new List<EmployeeShedule>();

			OrganisationShedules = new ObservableCollection<OrganisationShedulesViewModel>();
			foreach (var organisation in organisations)
			{
				var sheduleViewModel = new OrganisationShedulesViewModel();
				sheduleViewModel.Initialize(organisation, new List<EmployeeShedule>(employeeShedules.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationShedules.Add(sheduleViewModel);
			}
			SelectedOrganisationShedule = OrganisationShedules.FirstOrDefault();
		}

		ObservableCollection<OrganisationShedulesViewModel> _organisationShedule;
		public ObservableCollection<OrganisationShedulesViewModel> OrganisationShedules
		{
			get { return _organisationShedule; }
			set
			{
				_organisationShedule = value;
				OnPropertyChanged("OrganisationShedules");
			}
		}

		OrganisationShedulesViewModel _selectedOrganisationShedule;
		public OrganisationShedulesViewModel SelectedOrganisationShedule
		{
			get { return _selectedOrganisationShedule; }
			set
			{
				_selectedOrganisationShedule = value;
				OnPropertyChanged("SelectedOrganisationShedule");
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}
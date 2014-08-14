﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalViewModel : BaseObjectViewModel<ScheduleDayInterval>
	{
		ScheduleSchemeViewModel _scheduleScheme;
		bool _initialized;

		public DayIntervalViewModel(ScheduleSchemeViewModel scheduleScheme, ScheduleDayInterval dayInterval)
			: base(dayInterval)
		{
			_initialized = false;
			_scheduleScheme = scheduleScheme;
			Update();
			_scheduleScheme.PropertyChanged += OrganisationViewModel_PropertyChanged;
			_initialized = true;
		}

		public string Name { get; private set; }
		public ObservableCollection<DayInterval> NamedIntervals
		{
			get { return _scheduleScheme.NamedIntervals; }
		}

		public override void Update()
		{
			base.Update();
			if (_scheduleScheme.ScheduleScheme.Type == ScheduleSchemeType.Week)
			{
				var dayOfWeek = (DayOfWeek)((Model.Number + 1) % 7);
				var culture = new System.Globalization.CultureInfo("ru-RU");
				Name = culture.DateTimeFormat.GetDayName(dayOfWeek);
			}
			else
			{
				Name = (Model.Number + 1).ToString();
			}
			//Name = _scheduleScheme.ScheduleScheme.Type == ScheduleSchemeType.Week ? ((DayOfWeek)((Model.Number + 1) % 7)).ToString("dddd") : (Model.Number + 1).ToString();
			SelectedNamedInterval = NamedIntervals.SingleOrDefault(item => item.UID == Model.DayIntervalUID) ?? NamedIntervals[0];
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => NamedIntervals);
		}

		DayInterval _selectedNamedInterval;
		public DayInterval SelectedNamedInterval
		{
			get { return _selectedNamedInterval; }
			set
			{
				if (SelectedNamedInterval != value)
				{
					_selectedNamedInterval = value ?? NamedIntervals[0];
					if (_initialized || Model.DayIntervalUID != _selectedNamedInterval.UID)
					{
						Model.DayIntervalUID = _selectedNamedInterval.UID;
						SheduleDayIntervalHelper.Save(Model);
					}
				}
				OnPropertyChanged(() => SelectedNamedInterval);
			}
		}

		void OrganisationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "NamedIntervals")
				Update();
		}
	}
}
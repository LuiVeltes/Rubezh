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

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalsViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public WeeklyIntervalsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			Initialize();
		}

		public void Initialize()
		{
			var employeeWeeklyIntervals = new List<EmployeeWeeklyInterval>();
			var neverTimeInterval = employeeWeeklyIntervals.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			if (neverTimeInterval == null)
			{
				neverTimeInterval = new EmployeeWeeklyInterval() { Name = "Никогда", IsDefault = true };
				employeeWeeklyIntervals.Add(neverTimeInterval);
			}

			WeeklyIntervals = new ObservableCollection<WeeklyIntervalViewModel>();
			foreach (var weeklyInterval in employeeWeeklyIntervals)
			{
				var timeInrervalViewModel = new WeeklyIntervalViewModel(weeklyInterval);
				WeeklyIntervals.Add(timeInrervalViewModel);
			}
			SelectedWeeklyInterval = WeeklyIntervals.FirstOrDefault();
		}

		EmployeeWeeklyInterval IntervalToCopy;

		ObservableCollection<WeeklyIntervalViewModel> _weeklyIntervals;
		public ObservableCollection<WeeklyIntervalViewModel> WeeklyIntervals
		{
			get { return _weeklyIntervals; }
			set
			{
				_weeklyIntervals = value;
				OnPropertyChanged("WeeklyIntervals");
			}
		}

		WeeklyIntervalViewModel _selectedWeeklyInterval;
		public WeeklyIntervalViewModel SelectedWeeklyInterval
		{
			get { return _selectedWeeklyInterval; }
			set
			{
				_selectedWeeklyInterval = value;
				OnPropertyChanged("SelectedWeeklyInterval");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = WeeklyIntervals.FirstOrDefault(x => x.WeeklyInterval.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedWeeklyInterval = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var weeklyIntervalDetailsViewModel = new WeeklyIntervalDetailsViewModel(this);
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				var timeInrervalViewModel = new WeeklyIntervalViewModel(weeklyIntervalDetailsViewModel.WeeklyInterval);
				WeeklyIntervals.Add(timeInrervalViewModel);
				SelectedWeeklyInterval = timeInrervalViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			WeeklyIntervals.Remove(SelectedWeeklyInterval);
		}
		bool CanDelete()
		{
			return SelectedWeeklyInterval != null && !SelectedWeeklyInterval.WeeklyInterval.IsDefault && WeeklyIntervals.Count > 2;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var weeklyIntervalDetailsViewModel = new WeeklyIntervalDetailsViewModel(this, SelectedWeeklyInterval.WeeklyInterval);
			if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
			{
				SelectedWeeklyInterval.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedWeeklyInterval != null && !SelectedWeeklyInterval.WeeklyInterval.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			IntervalToCopy = CopyInterval(SelectedWeeklyInterval.WeeklyInterval);
		}
		bool CanCopy()
		{
			return SelectedWeeklyInterval != null && !SelectedWeeklyInterval.WeeklyInterval.IsDefault;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newInterval = CopyInterval(IntervalToCopy);
			var timeInrervalViewModel = new WeeklyIntervalViewModel(newInterval);
			WeeklyIntervals.Add(timeInrervalViewModel);
			SelectedWeeklyInterval = timeInrervalViewModel;
		}
		bool CanPaste()
		{
			return IntervalToCopy != null;
		}

		EmployeeWeeklyInterval CopyInterval(EmployeeWeeklyInterval source)
		{
			var copy = new EmployeeWeeklyInterval();
			copy.Name = source.Name;
			foreach (var weeklyIntervalPart in source.WeeklyIntervalParts)
			{
				var copyWeeklyIntervalPart = new EmployeeWeeklyIntervalPart()
				{
					No = weeklyIntervalPart.No,
					TimeIntervalUID = weeklyIntervalPart.TimeIntervalUID,
				};
				copy.WeeklyIntervalParts.Add(copyWeeklyIntervalPart);
			}
			return copy;
		}
	}
}
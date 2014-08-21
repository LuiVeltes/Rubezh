﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalViewModel : BaseIntervalViewModel<TimeIntervalPartViewModel, SKDTimeInterval>
	{
		public TimeIntervalViewModel(int index, SKDTimeInterval timeInterval)
			: base(index, timeInterval)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);
			InitParts();
			Update();
		}

		public override void Update()
		{
			base.Update();
			Name = IsActive ? Model.Name : string.Format("Дневной график {0}", Index);
			Description = IsActive ? Model.Description : string.Empty;
		}
		protected override void Activate()
		{
			if (IsActive && Model == null)
			{
				Model = new SKDTimeInterval()
				{
					ID = Index,
					Name = Name,
					TimeIntervalParts = new List<SKDTimeIntervalPart>(),
				};
				InitParts();
				SKDManager.TimeIntervalsConfiguration.TimeIntervals.Add(Model);
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
			else if (!IsActive && Model != null)
			{
				if (ConfirmDeactivation())
				{
					SKDManager.TimeIntervalsConfiguration.TimeIntervals.Remove(Model);
					Model = null;
					InitParts();
					SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.ForEach(week => week.InvalidateDayIntervals());
					SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.ForEach(week => week.InvalidateDayIntervals());
					ServiceFactory.SaveService.SKDChanged = true;
					ServiceFactory.SaveService.TimeIntervalChanged();
				}
				else
					IsActive = true;
			}
			base.Activate();
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			Edit(null);
		}
		private bool CanAdd()
		{
			return Parts.Count < 4;
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			Model.TimeIntervalParts.Remove(SelectedPart.TimeIntervalPart);
			Parts.Remove(SelectedPart);
			ServiceFactory.SaveService.SKDChanged = true;
			ServiceFactory.SaveService.TimeIntervalChanged();
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			Edit(SelectedPart);
		}
		private bool CanEdit()
		{
			return SelectedPart != null;
		}

		public override void Paste(SKDTimeInterval timeInterval)
		{
			IsActive = true;
			Model.TimeIntervalParts = timeInterval.TimeIntervalParts;
			InitParts();
			ServiceFactory.SaveService.SKDChanged = true;
			ServiceFactory.SaveService.TimeIntervalChanged();
			Update();
		}
		private void InitParts()
		{
			Parts = new ObservableCollection<TimeIntervalPartViewModel>();
			if (Model != null)
				foreach (var timeIntervalPart in Model.TimeIntervalParts)
				{
					var timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPart);
					Parts.Add(timeIntervalPartViewModel);
				}
		}

		private void Edit(TimeIntervalPartViewModel timeIntervalPartViewModel)
		{
			var timeIntervalPartDetailsViewModel = new TimeIntervalPartDetailsViewModel(timeIntervalPartViewModel == null ? null : timeIntervalPartViewModel.TimeIntervalPart);
			if (DialogService.ShowModalWindow(timeIntervalPartDetailsViewModel))
			{
				if (timeIntervalPartViewModel == null)
				{
					Model.TimeIntervalParts.Add(timeIntervalPartDetailsViewModel.TimeIntervalPart);
					timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPartDetailsViewModel.TimeIntervalPart);
					Parts.Add(timeIntervalPartViewModel);
					SelectedPart = timeIntervalPartViewModel;
				}
				SelectedPart.Update();
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
		}

		private bool ConfirmDeactivation()
		{
			var hasReference = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Any(item => item.WeeklyIntervalParts.Any(part => part.TimeIntervalID == Index));
			if (!hasReference)
				hasReference = SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.Any(item => item.TimeIntervalIDs.Contains(Index));
			return !hasReference || MessageBoxService.ShowConfirmation2("Данный дневной график используется в одном или нескольких недельных графиках или скользящих посуточных графиках, Вы уверены что хотите его деактивировать?");
		}
	}
}
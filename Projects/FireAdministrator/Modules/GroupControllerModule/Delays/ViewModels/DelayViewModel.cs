﻿using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DelayViewModel : BaseViewModel
	{
		public GKDelay Delay { get; set; }

		public DelayViewModel(GKDelay delay)
		{
			ShowLogicCommand = new RelayCommand(OnShowLogic);
			Delay = delay;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Delay);
		}

		public string PresentationLogic
		{
			get
			{
				var presentationZone = GKManager.GetPresentationZone(Delay.Logic);
				IsZoneGrayed = string.IsNullOrEmpty(presentationZone);
				if (string.IsNullOrEmpty(presentationZone))
				{
					presentationZone = "Нажмите для настройки логики";
				}
				return presentationZone;
			}
		}

		bool _isZoneGrayed;
		public bool IsZoneGrayed
		{
			get { return _isZoneGrayed; }
			set
			{
				_isZoneGrayed = value;
				OnPropertyChanged(() => IsZoneGrayed);
			}
		}

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			DelaysViewModel.Current.SelectedDelay = this;
			var logicViewModel = new LogicViewModel(null, Delay.Logic, true);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				Delay.Logic = logicViewModel.GetModel();
				OnPropertyChanged(() => PresentationLogic);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}
﻿using AlarmModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using AlarmModule.Events;
using FiresecAPI.Models;
using Infrastructure.Common.Navigation;
using System.Collections.Generic;

namespace AlarmModule
{
	public class AlarmModuleLoader : ModuleBase
	{
		AlarmWatcher AlarmWatcher;
		AlarmVideoWather AlarmVideoWather;
		AlarmsViewModel AlarmsViewModel;

		public AlarmModuleLoader()
		{
			ServiceFactory.Layout.AddAlarmGroups(new AlarmGroupListViewModel());
			ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Subscribe(OnShowAlarms);
		}

		void CreateViewModels()
		{
			AlarmsViewModel = new AlarmsViewModel();
		}

		void CreateWatchers()
		{
			AlarmWatcher = new AlarmWatcher();
			AlarmVideoWather = new AlarmVideoWather();
		}

		void OnShowAlarms(AlarmType? alarmType)
		{
			AlarmsViewModel.Sort(alarmType);
			ServiceFactory.Layout.Show(AlarmsViewModel);
		}
		
		public override void Initialize()
		{
			CreateViewModels();
			CreateWatchers();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowAlarmsEvent, AlarmType?>("Тревоги", "/Controls;component/Images/Alarm.png")
			};
		}
	}
}
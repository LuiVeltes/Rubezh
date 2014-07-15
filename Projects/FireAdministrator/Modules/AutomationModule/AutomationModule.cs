﻿using System;
using System.Collections.Generic;
using AutomationModule.Events;
using AutomationModule.Validation;
using AutomationModule.ViewModels;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using FiresecClient;
using FiresecAPI.Automation;

namespace AutomationModule
{
	public class AutomationModule : ModuleBase, IValidationModule
	{
		SoundsViewModel SoundsViewModel;
		ProceduresViewModel ProceduresViewModel;
		SchedulesViewModel SchedulesViewModel;
		GlobalVariablesViewModel GlobalVariablesViewModel;

		public override void CreateViewModels()
		{
			//FiresecManager.SystemConfiguration.AutomationConfiguration = new AutomationConfiguration();
			SoundsViewModel = new SoundsViewModel();
			ProceduresViewModel = new ProceduresViewModel();
			SchedulesViewModel = new SchedulesViewModel();
			GlobalVariablesViewModel = new GlobalVariablesViewModel();
		}

		public override void Initialize()
		{
			SoundsViewModel.Initialize();
			ProceduresViewModel.Initialize();
			SchedulesViewModel.Initialize();
			GlobalVariablesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>
				{
					new NavigationItem("Автоматизация", "/Controls;component/Images/tree.png",
						new List<NavigationItem>()
						{
							new NavigationItem<ShowProceduresEvent, Guid>(ProceduresViewModel, "Процедуры", "/Controls;component/Images/Procedures.png"),
							new NavigationItem<ShowAutomationSchedulesEvents, Guid>(SchedulesViewModel, "Расписания", "/Controls;component/Images/Shedules.png"),
							new NavigationItem<ShowGlobalVariablesEvent, Guid>(GlobalVariablesViewModel, "Глобальные переменные", "/Controls;component/Images/GlobalVariables.png"),
							new NavigationItem<ShowAutomationSoundsEvent, Guid>(SoundsViewModel, "Звуки", "/Controls;component/Images/Music.png")
						}) {IsExpanded = true},
				};
		}
		public override string Name
		{
			get { return "Автоматизация"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Sounds/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Procedures/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "Schedules/DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(GetType().Assembly, "GlobalVariables/DataTemplates/Dictionary.xaml"));
		}

		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}
	}
}
﻿using System;
using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using OPCModule.Validation;
using OPCModule.ViewModels;

namespace OPCModule
{
    public class OPCModule : ModuleBase, IValidationModule
	{
		OPCDevicesViewModel OPCDevicesViewModel;
		OPCZonesViewModel OPCZonesViewModel;

		public override void CreateViewModels()
		{
			OPCDevicesViewModel = new OPCDevicesViewModel();
			OPCZonesViewModel = new OPCZonesViewModel();
		}

		public override void Initialize()
		{
			OPCDevicesViewModel.Initialize();
			OPCZonesViewModel.Initialize();
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem("OPC сервер", "/Controls;component/Images/Tree.png", new List<NavigationItem>()
				{
					new NavigationItem<ShowOPCDeviceEvent, Guid>(OPCDevicesViewModel, "Устройства","/Controls;component/Images/Tree.png", null, null, Guid.Empty),
					new NavigationItem<ShowOPCZoneEvent, Guid>(OPCZonesViewModel, "Зоны","/Controls;component/Images/Zones.png", null, null, Guid.Empty),
				}),
			};
		}
		public override string Name
		{
			get { return "OPC сервер"; }
		}

        #region IValidationModule Members
        public IEnumerable<IValidationError> Validate()
        {
            return Validator.Validate();
        }
        #endregion
	}
}
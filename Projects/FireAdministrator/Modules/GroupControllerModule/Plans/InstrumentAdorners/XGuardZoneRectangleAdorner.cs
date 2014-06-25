﻿using FiresecAPI.Models;
using GKModule.Plans.Designer;
using GKModule.Plans.ViewModels;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.InstrumentAdorners;

namespace GKModule.Plans.InstrumentAdorners
{
	public class XGuardZoneRectangleAdorner : BaseRectangleAdorner
	{
		GuardZonesViewModel _guardZonesViewModel;

		public XGuardZoneRectangleAdorner(CommonDesignerCanvas designerCanvas, GuardZonesViewModel guardZonesViewModel)
			: base(designerCanvas)
		{
			_guardZonesViewModel = guardZonesViewModel;
		}

		protected override Infrustructure.Plans.Elements.ElementBaseRectangle CreateElement()
		{
			var element = new ElementRectangleXGuardZone();
			var propertiesViewModel = new GuardZonePropertiesViewModel(element, _guardZonesViewModel);
			if (!DialogService.ShowModalWindow(propertiesViewModel))
				return null;
			Helper.SetXGuardZone(element);
			return element;
		}
	}
}
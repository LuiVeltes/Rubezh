﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Common;
using DeviceControls;
using DevicesModule.Plans.Designer;
using DevicesModule.Plans.InstrumentAdorners;
using DevicesModule.Plans.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using Devices = DevicesModule.ViewModels;

namespace DevicesModule.Plans
{
	public class PlanExtension : IPlanExtension<Plan>
	{
		private DevicesViewModel _devicesViewModel;
		private CommonDesignerCanvas _designerCanvas;
		public PlanExtension(Devices.DevicesViewModel devicesViewModel)
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(x => { UpdateDeviceInZones(); });
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(x => { UpdateDeviceInZones(); });

			_devicesViewModel = new DevicesViewModel(devicesViewModel);
		}

		public void Initialize()
		{
			DevicePictureCache.LoadCache();
		}

		#region IPlanExtension Members

		public int Index
		{
			get { return 0; }
		}
		public string Title
		{
			get { return "Устройства"; }
		}

		public object TabPage
		{
			get { return _devicesViewModel; }
		}

		public IEnumerable<IInstrument> Instruments
		{
			get
			{
				return new List<IInstrument>()
					{
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZoneRectangle.png",
							ToolTip="Зона",
							Adorner = new ZoneRectangleAdorner(_designerCanvas),
							Autostart = true
						},
						new InstrumentViewModel()
						{
							ImageSource="/Controls;component/Images/ZonePolygon.png",
							ToolTip="Зона",
							Adorner = new ZonePolygonAdorner(_designerCanvas),
							Autostart = true
						},
					};
			}
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			IElementZone elementZone = element as IElementZone;
			if (elementZone != null)
			{
				if (elementZone is ElementRectangleZone)
					plan.ElementRectangleZones.Add((ElementRectangleZone)elementZone);
				else if (elementZone is ElementPolygonZone)
					plan.ElementPolygonZones.Add((ElementPolygonZone)elementZone);
				else
					return false;
				return true;
			}
			else if (element is ElementDevice)
			{
				ElementDevice elementDevice = element as ElementDevice;
				Designer.Helper.SetDevice(elementDevice);
				plan.ElementDevices.Add(elementDevice);
				return true;
			}
			return false;
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
		{
			IElementZone elementZone = element as IElementZone;
			if (elementZone != null)
			{
				if (elementZone is ElementRectangleZone)
					plan.ElementRectangleZones.Remove((ElementRectangleZone)elementZone);
				else if (elementZone is ElementPolygonZone)
					plan.ElementPolygonZones.Remove((ElementPolygonZone)elementZone);
				else
					return false;
				return true;
			}
			else
			{
				ElementDevice elementDevice = element as ElementDevice;
				if (elementDevice == null)
					return false;
				else
				{
					plan.ElementDevices.Remove(elementDevice);
					return true;
				}
			}
		}

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementRectangleZone || designerItem.Element is ElementPolygonZone)
			{
				designerItem.ItemPropertyChanged += new EventHandler(ZonePropertyChanged);
				OnZonePropertyChanged(designerItem);
				designerItem.Group = "Zone";
				designerItem.UpdateProperties += UpdateDesignerItemZone;
				UpdateDesignerItemZone(designerItem);
			}
			else if (designerItem.Element is ElementDevice)
			{
				designerItem.ItemPropertyChanged += new EventHandler(DevicePropertyChanged);
				OnDevicePropertyChanged(designerItem);
				designerItem.Group = "Devices";
				designerItem.UpdateProperties += UpdateDesignerItemDevice;
				UpdateDesignerItemDevice(designerItem);
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			foreach (var element in plan.ElementDevices)
				yield return element;
			foreach (var element in plan.ElementRectangleZones)
				yield return element;
			foreach (var element in plan.ElementPolygonZones)
				yield return element;
		}

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup("Devices", "Устройства", 0);
			LayerGroupService.Instance.RegisterGroup("Zone", "Зоны", 1);
			UpdateDeviceInZones();
		}

		#endregion

		private void UpdateDesignerItemDevice(CommonDesignerItem designerItem)
		{
			ElementDevice elementDevice = designerItem.Element as ElementDevice;
			Device device = Designer.Helper.GetDevice(elementDevice);
			if (device == null)
				Designer.Helper.SetDevice(elementDevice, device);
			designerItem.Title = Designer.Helper.GetDeviceTitle((ElementDevice)designerItem.Element);
		}
		private void UpdateDesignerItemZone(CommonDesignerItem designerItem)
		{
			IElementZone elementZone = designerItem.Element as IElementZone;
			Zone zone = Designer.Helper.GetZone(elementZone);
			if (zone == null)
				Designer.Helper.SetZone(elementZone, zone);
			designerItem.Title = Designer.Helper.GetZoneTitle(zone);
			elementZone.BackgroundColor = Designer.Helper.GetZoneColor(zone);
			if (zone == null)
				elementZone.SetZLayer(2);
			else
				switch (zone.ZoneType)
				{
					case ZoneType.Fire:
						elementZone.SetZLayer(3);
						break;
					case ZoneType.Guard:
						elementZone.SetZLayer(4);
						break;
				}
		}

		private void ZonePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnZonePropertyChanged(designerItem);
		}
		private void OnZonePropertyChanged(DesignerItem designerItem)
		{
			Zone zone = Designer.Helper.GetZone((IElementZone)designerItem.Element);
			if (zone != null)
				zone.ColorTypeChanged += () =>
				{
					UpdateDesignerItemZone(designerItem);
					designerItem.Redraw();
				};
		}

		private void DevicePropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnDevicePropertyChanged(designerItem);
		}
		private void OnDevicePropertyChanged(DesignerItem designerItem)
		{
			Device device = Designer.Helper.GetDevice((ElementDevice)designerItem.Element);
			if (device != null)
				device.Changed += () =>
				{
					UpdateDesignerItemDevice(designerItem);
					designerItem.Redraw();
				};
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			if (args.Element is ElementDevice)
				args.Painter = new Painter();
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementDevice element = e.Element as ElementDevice;
			if (element != null)
				e.PropertyViewModel = new DevicePropertiesViewModel(_devicesViewModel, element);
			else if (e.Element is ElementRectangleZone || e.Element is ElementPolygonZone)
				e.PropertyViewModel = new ZonePropertiesViewModel((IElementZone)e.Element);
		}

		public void UpdateDeviceInZones()
		{
			var deviceInZones = new Dictionary<Device, Guid>();
			using (new WaitWrapper())
			using (new TimeCounter("\tUpdateDeviceInZones: {0}"))
				//foreach (var designerItem in _designerCanvas.Items)
				//{
				//    ElementDevice elementDevice = designerItem.Element as ElementDevice;
				//    if (elementDevice != null)
				//    {
				//        //var designerItemCenterX = Canvas.GetLeft(designerItem) + designerItem.Width / 2;
				//        //var designerItemCenterY = Canvas.GetTop(designerItem) + designerItem.Height / 2;
				//        var designerItemCenterX = elementDevice.Left;
				//        var designerItemCenterY = elementDevice.Top;
				//        var device = Designer.Helper.GetDevice(elementDevice);
				//        if (device == null || device.Driver == null || !device.Driver.IsZoneDevice)
				//            continue;
				//        var zoneUIDs = new List<Guid>();
				//        foreach (var elementPolygonZoneItem in _designerCanvas.Items)
				//        {
				//            var point = new Point((int)(designerItemCenterX - Canvas.GetLeft(elementPolygonZoneItem)), (int)(designerItemCenterY - Canvas.GetTop(elementPolygonZoneItem)));
				//            ElementPolygonZone elementPolygonZone = elementPolygonZoneItem.Element as ElementPolygonZone;
				//            if (elementPolygonZone != null)
				//            {
				//                bool isInPolygon = DesignerHelper.IsPointInPolygon(point, elementPolygonZoneItem.Presenter as Polygon);
				//                if (isInPolygon && elementPolygonZone.ZoneUID != Guid.Empty)
				//                    zoneUIDs.Add(elementPolygonZone.ZoneUID);
				//            }
				//            ElementRectangleZone elementRectangleZone = elementPolygonZoneItem.Element as ElementRectangleZone;
				//            if (elementRectangleZone != null)
				//            {
				//                bool isInRectangle = ((point.X > 0) && (point.X < elementRectangleZone.Width) && (point.Y > 0) && (point.Y < elementRectangleZone.Height));
				//                if (isInRectangle && elementRectangleZone.ZoneUID != Guid.Empty)
				//                    zoneUIDs.Add(elementRectangleZone.ZoneUID);
				//            }
				//        }

				//        if (device.ZoneUID != Guid.Empty)
				//        {
				//            var isInZone = zoneUIDs.Any(x => x == device.ZoneUID);
				//            if (!isInZone)
				//            {
				//                if (!deviceInZones.ContainsKey(device))
				//                    deviceInZones.Add(device, zoneUIDs.Count > 0 ? zoneUIDs[0] : Guid.Empty);
				//                else if (zoneUIDs.Count > 0)
				//                    deviceInZones[device] = zoneUIDs[0];
				//            }
				//        }
				//        else if (zoneUIDs.Count > 0)
				//        {
				//            var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUIDs[0]);
				//            if (zone != null)
				//            {
				//                FiresecManager.FiresecConfiguration.AddDeviceToZone(device, zone);
				//            }
				//        }
				//    }
				//}
			if (deviceInZones.Count > 0)
			{
				var deviceInZoneViewModel = new DevicesInZoneViewModel(deviceInZones);
				var result = DialogService.ShowModalWindow(deviceInZoneViewModel);
			}
		}
	}
}

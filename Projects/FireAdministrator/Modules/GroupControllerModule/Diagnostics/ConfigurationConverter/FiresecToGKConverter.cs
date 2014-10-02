﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure.Common.Windows;

namespace GKModule.Converter
{
	public class FiresecToGKConverter
	{
		GKDevice gkControllerDevice;

		public void Convert()
		{
			PrepareKAU();

			var convertationViewModel = new ConvertationViewModel();
			if (DialogService.ShowModalWindow(convertationViewModel))
			{
				foreach (var fsShleif in convertationViewModel.FSShleifs)
				{
					foreach (var childDevice in fsShleif.FSDevice.Children)
					{
						if (childDevice.IntAddress / 256 == fsShleif.FSShleifNo)
							AddDevice(childDevice, fsShleif.KAUDevice, (byte)fsShleif.KAUShleifNo);
					}
				}
				ConvertZones();
				ConvertLogic();
				GKManager.UpdateConfiguration();
			}
		}

		void PrepareKAU()
		{
			AddRootDevices();
			var totalShleifCount = 0;
			foreach (var panelDevice in GetPanels())
			{
				totalShleifCount += panelDevice.Driver.ShleifCount;
			}
			var kauCount = (totalShleifCount + 1) / 8 + Math.Min(1, totalShleifCount % 8);
			for (int i = 0; i < kauCount; i++)
			{
				GKManager.AddChild(gkControllerDevice, null, GKManager.Drivers.FirstOrDefault(x => x.IsKauOrRSR2Kau), (byte)(i + 1));
			}
			GKManager.UpdateConfiguration();
		}

		public void AddRootDevices()
		{
			GKManager.DeviceConfiguration = new GKDeviceConfiguration();

			var systemDevice = new GKDevice()
			{
				UID = Guid.NewGuid(),
				DriverUID = GKDriver.System_UID,
				Driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System)
			};
			GKManager.DeviceConfiguration.Devices.Add(systemDevice);
			GKManager.DeviceConfiguration.RootDevice = systemDevice;

			var gkDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			gkControllerDevice = GKManager.AddChild(systemDevice, null, gkDriver, 1);
			gkControllerDevice.UID = Guid.NewGuid();
			GKManager.DeviceConfiguration.Devices.Add(gkControllerDevice);
		}

		IEnumerable<Device> GetPanels()
		{
			foreach (var device in FiresecManager.Devices)
			{
				if (device.Driver.IsPanel)
					yield return device;
			}
		}

		public void AddDevice(Device fsDevice, GKDevice kauDevice, byte shleifNo)
		{
			var driver = GKManager.Drivers.FirstOrDefault(x => x.UID == fsDevice.DriverUID);
			if (driver == null)
			{
				return;
			}

			var device = new GKDevice()
			{
				UID = fsDevice.UID,
				DriverUID = driver.UID,
				Driver = driver,
				IntAddress = (byte)(fsDevice.IntAddress & 0xff),
				Description = fsDevice.Description
			};
			GKManager.DeviceConfiguration.Devices.Add(device);
			var shleifDevice = kauDevice.Children.FirstOrDefault(x => x.ShleifNo == shleifNo);
			if (shleifDevice != null)
			{
				shleifDevice.Children.Add(device);
				device.Parent = shleifDevice;
			}

			foreach (var fsChildDevice in fsDevice.Children)
			{
				AddDevice(fsChildDevice, device, shleifNo);
			}
		}

		void ConvertZones()
		{
			foreach (var zone in FiresecManager.Zones)
			{
				var xZone = new GKZone()
				{
					UID = zone.UID,
					No = (ushort)zone.No,
					Name = zone.Name,
					Description = zone.Description,
					Fire1Count = (ushort)zone.DetectorCount,
				};
				GKManager.Zones.Add(xZone);
			}

			foreach (var device in FiresecManager.Devices)
			{
				var xDevice = GKManager.Devices.FirstOrDefault(x => x.UID == device.UID);
				if (xDevice != null)
				{
					if ((device.Driver.IsZoneDevice) && (device.ZoneUID != Guid.Empty) && (device.Driver.DriverType != DriverType.MPT))
					{
						var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == device.ZoneUID);
						var xZone = GKManager.Zones.FirstOrDefault(x => x.No == (ushort)zone.No);
						if (zone != null)
						{
							xDevice.ZoneUIDs.Add(xZone.UID);
						}
					}
				}
			}
		}

		void ConvertLogic()
		{
			foreach (var xDevice in GKManager.Devices)
			{
				var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == xDevice.UID);
				if (device != null)
				{
					if ((device.Driver.IsZoneLogicDevice) && (device.ZoneLogic != null))
					{
						var xDeviceLogic = new GKDeviceLogic();

						foreach (var clause in device.ZoneLogic.Clauses)
						{
							var xClause = new GKClause()
							{
								ClauseOperationType = ClauseOperationType.AllZones
							};
							if (clause.Operation.HasValue)
								switch (clause.Operation.Value)
								{
									case ZoneLogicOperation.All:
										xClause.ClauseOperationType = ClauseOperationType.AllZones;
										break;

									case ZoneLogicOperation.Any:
										xClause.ClauseOperationType = ClauseOperationType.AnyZone;
										break;
								}
							switch (clause.State)
							{
								case FiresecAPI.Models.ZoneLogicState.Attention:
									xClause.StateType = GKStateBit.Attention;
									break;

								case FiresecAPI.Models.ZoneLogicState.Fire:
									xClause.StateType = GKStateBit.Fire1;
									break;

								case FiresecAPI.Models.ZoneLogicState.Failure:
									xClause.StateType = GKStateBit.Failure;
									break;

								default:
									continue;
							}
							if ((clause.ZoneUIDs == null) || (clause.ZoneUIDs.Count == 0))
								continue;

							foreach (var zoneUID in clause.ZoneUIDs)
							{
								var xZone = GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
								xClause.ZoneUIDs.Add(xZone.UID);
							}

							xDeviceLogic.ClausesGroup.Clauses.Add(xClause);
						}

						if (xDeviceLogic.ClausesGroup.Clauses.Count > 0)
							xDevice.DeviceLogic = xDeviceLogic;
					}
					if (device.Driver.DriverType == DriverType.MPT)
					{
						if (device.Zone != null)
						{
							var xClause = new GKClause();
							xClause.ClauseOperationType = ClauseOperationType.AnyZone;
							xClause.ZoneUIDs.Add(device.Zone.UID);
							xDevice.DeviceLogic.ClausesGroup.Clauses.Add(xClause);
						}
					}
				}
			}
		}
	}
}
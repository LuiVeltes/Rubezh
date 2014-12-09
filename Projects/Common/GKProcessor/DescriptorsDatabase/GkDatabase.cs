﻿using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class GkDatabase : CommonDatabase
	{
		List<GKDevice> Devices { get; set; }
		List<GKZone> Zones { get; set; }
		List<GKGuardZone> GuardZones { get; set; }
		List<GKDirection> Directions { get; set; }
		List<GKPumpStation> PumpStations { get; set; }
		List<GKMPT> MPTs { get; set; }
		public List<GKDelay> Delays { get; private set; }
		public List<GKPim> Pims { get; private set; }
		List<GKCode> Codes { get; set; }
		List<GKDoor> Doors { get; set; }
		public List<KauDatabase> KauDatabases { get; set; }

		public GkDatabase(GKDevice gkControllerDevice)
		{
			Devices = new List<GKDevice>();
			Zones = new List<GKZone>();
			GuardZones = new List<GKGuardZone>();
			Directions = new List<GKDirection>();
			PumpStations = new List<GKPumpStation>();
			MPTs = new List<GKMPT>();
			Delays = new List<GKDelay>();
			Pims = new List<GKPim>();
			Codes = new List<GKCode>();
			Doors = new List<GKDoor>();
			KauDatabases = new List<KauDatabase>();
			DatabaseType = DatabaseType.Gk;
			RootDevice = gkControllerDevice;

			AddDevice(gkControllerDevice);
			foreach (var device in gkControllerDevice.Children)
			{
				if (device.DriverType == GKDriverType.GKIndicator || device.DriverType == GKDriverType.GKRele)
				{
					AddDevice(device);
				}
			}
			Devices.ForEach(x => x.GkDatabaseParent = RootDevice);
		}

		void AddDevice(GKDevice device)
		{
			if (!Devices.Contains(device))
			{
				device.GKDescriptorNo = NextDescriptorNo;
				Devices.Add(device);
			}
		}

		public void AddDelay(GKDelay delay)
		{
			if (!Delays.Contains(delay))
			{
				delay.GKDescriptorNo = NextDescriptorNo;
				delay.GkDatabaseParent = RootDevice;
				Delays.Add(delay);
			}
		}

		public void AddPim(GKPim pim)
		{
			if (!Pims.Contains(pim))
			{
				pim.GKDescriptorNo = NextDescriptorNo;
				pim.GkDatabaseParent = RootDevice;
				Pims.Add(pim);
			}
		}

		public override void BuildObjects()
		{
			AddKauObjects();
			foreach (var zone in GKManager.Zones)
			{
				if (zone.GkDatabaseParent == RootDevice)
				{
					zone.GKDescriptorNo = NextDescriptorNo;
					Zones.Add(zone);
				}
			}
			foreach (var guardZone in GKManager.GuardZones)
			{
				if (guardZone.GkDatabaseParent == RootDevice)
				{
					guardZone.GKDescriptorNo = NextDescriptorNo;
					GuardZones.Add(guardZone);
				}
			}
			foreach (var direction in GKManager.Directions)
			{
				if (direction.GkDatabaseParent == RootDevice)
				{
					direction.GKDescriptorNo = NextDescriptorNo;
					Directions.Add(direction);
				}
			}
			foreach (var pumpStation in GKManager.PumpStations)
			{
				if (pumpStation.GkDatabaseParent == RootDevice)
				{
					PumpStations.Add(pumpStation);
				}
			}

			foreach (var mpt in GKManager.DeviceConfiguration.MPTs)
			{
				if (mpt.GkDatabaseParent == RootDevice)
				{
					MPTs.Add(mpt);
				}
			}

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (code.GkDatabaseParent == RootDevice)
				{
					code.GKDescriptorNo = NextDescriptorNo;
					Codes.Add(code);
				}
			}

			foreach (var door in GKManager.Doors)
			{
				if (door.GkDatabaseParent == RootDevice)
				{
					door.GKDescriptorNo = NextDescriptorNo;
					Doors.Add(door);
				}
			}

			Descriptors = new List<BaseDescriptor>();
			foreach (var device in Devices)
			{
				var deviceDescriptor = new DeviceDescriptor(device, DatabaseType);
				Descriptors.Add(deviceDescriptor);
			}
			foreach (var zone in Zones)
			{
				var zoneDescriptor = new ZoneDescriptor(zone, DatabaseType.Gk);
				Descriptors.Add(zoneDescriptor);
			}
			foreach (var guardZone in GuardZones)
			{
				var guardZoneDescriptor = new GuardZoneDescriptor(this, guardZone);
				Descriptors.Add(guardZoneDescriptor);

				guardZoneDescriptor.Build();
				if (guardZoneDescriptor.GuardZonePim != null)
				{
					AddPim(guardZoneDescriptor.GuardZonePim);
					Descriptors.Add(guardZoneDescriptor.GuardZonePimDescriptor);
				}
			}
			foreach (var direction in Directions)
			{
				var directionDescriptor = new DirectionDescriptor(direction);
				Descriptors.Add(directionDescriptor);
			}

			foreach (var pumpStation in PumpStations)
			{
				pumpStation.GKDescriptorNo = NextDescriptorNo;
				var pumpStationDescriptor = new PumpStationDescriptor(this, pumpStation);
				Descriptors.Add(pumpStationDescriptor);

				var pumpStationCreator = new PumpStationCreator(this, pumpStation, pumpStationDescriptor.MainDelay);
				pumpStationCreator.Create();
			}

			foreach (var mpt in MPTs)
			{
				mpt.GKDescriptorNo = NextDescriptorNo;
				var mptDescriptor = new MPTDescriptor(this, mpt);
				Descriptors.Add(mptDescriptor);

				var mptCreator = new MPTCreator(this, mpt, mptDescriptor.HandAutomaticOffPim, mptDescriptor.DoorAutomaticOffPim, mptDescriptor.FailureAutomaticOffPim);
				mptCreator.Create();
			}

			foreach (var delay in GKManager.Delays)
			{
				if (delay.GkDatabaseParent == RootDevice)
				{
					delay.GKDescriptorNo = NextDescriptorNo;
					delay.IsAutoGenerated = false;
					Delays.Add(delay);

					var delayDescriptor = new DelayDescriptor(delay);
					Descriptors.Add(delayDescriptor);
				}
			}

			foreach (var code in Codes)
			{
				var codeDescriptor = new CodeDescriptor(code);
				Descriptors.Add(codeDescriptor);
			}

			foreach (var door in Doors)
			{
				var doorDescriptor = new DoorDescriptor(door);
				Descriptors.Add(doorDescriptor);
			}

			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				descriptor.InitializeAllBytes();
			}
		}

		void AddKauObjects()
		{
			foreach (var kauDatabase in KauDatabases)
			{
				foreach (var descriptor in kauDatabase.Descriptors)
				{
					var gkBase = descriptor.GKBase;
					gkBase.GkDatabaseParent = RootDevice;
					if (gkBase is GKDevice)
					{
						GKDevice device = gkBase as GKDevice;
						AddDevice(device);
					}
				}
			}
		}
	}
}
﻿using System.Collections.Generic;
using Common;
using Infrastructure;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using System.Threading;
using System;
using System.Linq;
using System.Diagnostics;
using FiresecClient;
using Infrastructure.Common.Services;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsAnyDBMissmatch = false;

		bool _isJournalAnyDBMissmatch = false;
		bool IsJournalAnyDBMissmatch
		{
			get { return _isJournalAnyDBMissmatch; }
			set
			{
				if (_isJournalAnyDBMissmatch != value)
				{
					_isJournalAnyDBMissmatch = value;
					var journalItem = new JournalItem()
					{
						SystemDateTime = DateTime.Now,
						DeviceDateTime = DateTime.Now,
						GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						JournalItemType = JournalItemType.System,
						StateClass = XStateClass.Unknown,
						ObjectStateClass = XStateClass.Norm,
						Name = value ? "База данных прибора соответствует базе данных ПК" : "База данных прибора не соответствует базе данных ПК"
					};
					AddJournalItem(journalItem);
				}
			}
		}

		void GetAllStates(bool showProgress)
		{
			IsAnyDBMissmatch = false;

			if (showProgress)
				GKProcessorManager.OnStartProgress("Опрос объектов ГК", GkDatabase.Descriptors.Count, false);
			foreach (var descriptor in GkDatabase.Descriptors)
			{
				LastUpdateTime = DateTime.Now;
				var result = GetState(descriptor.XBase);
				if (!result)
				{
					if (descriptor.Device != null && descriptor.Device.DriverType == XDriverType.GK)
					{
						descriptor.Device.DeviceState.IsConnectionLost = true;
						break;
					}
				}
				if (showProgress)
					GKProcessorManager.OnDoProgress(descriptor.XBase.DescriptorInfo);
			}
			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.KAU_Shleif || device.DriverType == XDriverType.RSR2_KAU_Shleif)
				{
					device.DeviceState.OnInternalStateChanged();
				}
			}
			if (showProgress)
				GKProcessorManager.OnStopProgress();

			if (IsAnyDBMissmatch)
			{
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					var baseState = descriptor.XBase.GetXBaseState();
					baseState.StateBits = new List<XStateBit>() { XStateBit.Norm };
					baseState.IsGKMissmatch = true;
				}
			}
			else
			{
				foreach (var descriptor in GkDatabase.Descriptors)
				{
					var baseState = descriptor.XBase.GetXBaseState();
					baseState.IsGKMissmatch = false;
				}
			}
			IsJournalAnyDBMissmatch = IsAnyDBMissmatch;
			CheckTechnologicalRegime();

			foreach(var device in XManager.Devices)
			{
				OnObjectStateChanged(device);
			}
			foreach (var zone in XManager.Zones)
			{
				OnObjectStateChanged(zone);
			}
			foreach (var direction in XManager.Directions)
			{
				OnObjectStateChanged(direction);
			}
			foreach (var pumpStation in XManager.PumpStations)
			{
				OnObjectStateChanged(pumpStation);
			}
		}

		bool GetState(XBase xBase)
		{
			var no = xBase.GKDescriptorNo;
			var sendResult = SendManager.Send(xBase.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return false;
			}
			if (sendResult.Bytes.Count != 68)
			{
				IsAnyDBMissmatch = true;
				xBase.GetXBaseState().IsGKMissmatch = true;
				return false;
			}
			ConnectionChanged(true);
			var descriptorStateHelper = new DescriptorStateHelper();
			descriptorStateHelper.Parse(sendResult.Bytes, xBase);
			CheckDBMissmatch(xBase, descriptorStateHelper);

			var binaryState = xBase.GetXBaseState();
			binaryState.LastDateTime = DateTime.Now;
			binaryState.AdditionalStates = descriptorStateHelper.AdditionalStates;
			binaryState.OnDelay = descriptorStateHelper.OnDelay;
			binaryState.HoldDelay = descriptorStateHelper.HoldDelay;
			binaryState.OffDelay = descriptorStateHelper.OffDelay;
			binaryState.StateBits = descriptorStateHelper.StateBits;
			return true;
		}

		void CheckAdditionalStates(BaseDescriptor descriptor)
		{
			if (descriptor is DeviceDescriptor)
			{
				var deviceDescriptor = descriptor as DeviceDescriptor;
				if (deviceDescriptor.Device.DriverType == XDriverType.GK || deviceDescriptor.Device.DriverType == XDriverType.KAU)
				{
					GetState(descriptor.XBase);
				}
			}
		}

		void CheckDBMissmatch(XBase xBase, DescriptorStateHelper descriptorStateHelper)
		{
			bool isMissmatch = false;
			if (xBase is XDevice)
			{
				var device = xBase as XDevice;
				if (device.Driver.DriverTypeNo != descriptorStateHelper.TypeNo)
					isMissmatch = true;

				ushort physicalAddress = device.IntAddress;
				if (device.Driver.IsDeviceOnShleif)
					physicalAddress = (ushort)((device.ShleifNo - 1) * 256 + device.IntAddress);
				if (device.DriverType != XDriverType.GK && device.DriverType != XDriverType.KAU && device.DriverType != XDriverType.RSR2_KAU
					&& device.Driver.HasAddress && physicalAddress != descriptorStateHelper.PhysicalAddress)
					isMissmatch = true;

				var nearestDescriptorNo = 0;
				if (device.KauDatabaseParent != null)
					nearestDescriptorNo = device.KAUDescriptorNo;
				else if (device.GkDatabaseParent != null)
					nearestDescriptorNo = device.GKDescriptorNo;
				if (nearestDescriptorNo != descriptorStateHelper.AddressOnController)
					isMissmatch = true;
			}
			if (xBase is XZone)
			{
				var zone = xBase as XZone;
				if (descriptorStateHelper.TypeNo != 0x100)
					isMissmatch = true;
			}
			if (xBase is XDirection)
			{
				var direction = xBase as XDirection;
				if (descriptorStateHelper.TypeNo != 0x106)
					isMissmatch = true;
			}
			if (xBase is XPumpStation)
			{
				var pumpStation = xBase as XPumpStation;
				if (descriptorStateHelper.TypeNo != 0x106)
					isMissmatch = true;
			}
			if (xBase is XDelay)
			{
				var delay = xBase as XDelay;
				if (descriptorStateHelper.TypeNo != 0x101)
					isMissmatch = true;
			}

			var description = xBase.GetDescriptorName();
			if (xBase.GetDescriptorName().TrimEnd(' ') != descriptorStateHelper.Description)
				isMissmatch = true;

			xBase.GetXBaseState().IsRealMissmatch = isMissmatch;
			if (isMissmatch)
			{
				IsAnyDBMissmatch = true;
			}
		}

		void CheckServiceRequired(XBase xBase, JournalItem journalItem)
		{
			if (journalItem.Name == "Запыленность" || journalItem.Name == "Запыленность устранена")
			{
				if (xBase is XDevice)
				{
					var device = xBase as XDevice;
					if (journalItem.Name == "Запыленность")
						device.DeviceState.IsService = true;
					if (journalItem.Name == "Запыленность устранена")
						device.DeviceState.IsService = false;
				}
			}
		}

		#region TechnologicalRegime
		void CheckTechnologicalRegime()
		{
			var isInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(GkDatabase.RootDevice);
			foreach (var descriptor in GkDatabase.Descriptors)
			{
				var baseState = descriptor.XBase.GetXBaseState();
				baseState.IsInTechnologicalRegime = isInTechnologicalRegime;
			}

			if (!isInTechnologicalRegime)
			{
				foreach (var kauDatabase in GkDatabase.KauDatabases)
				{
					isInTechnologicalRegime = DeviceBytesHelper.IsInTechnologicalRegime(kauDatabase.RootDevice);
					var allChildren = XManager.GetAllDeviceChildren(kauDatabase.RootDevice);
					allChildren.Add(kauDatabase.RootDevice);
					foreach (var device in allChildren)
					{
						var baseState = device.GetXBaseState();
						baseState.IsInTechnologicalRegime = isInTechnologicalRegime;
					}
				}
			}
		}
		#endregion
	}
}
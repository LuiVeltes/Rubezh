﻿using System.Collections.Generic;
using Common;
using Common.GK;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule
{
    public partial class Watcher
	{
        void GetAllStates()
        {
			StartProgress("Опрос объектов ГК", GkDatabase.BinaryObjects.Count);
            foreach (var binaryObject in GkDatabase.BinaryObjects)
            {
                bool result = GetState(binaryObject.BinaryBase, GkDatabase.RootDevice);
				if (!result)
				{
					if (binaryObject.Device != null && binaryObject.Device.Driver.DriverType == XDriverType.GK)
					{
						binaryObject.Device.DeviceState.IsConnectionLost = true;
						break;
					}
				}
				DoProgress("Опрос объекта ГК " + binaryObject.BinaryBase.BinaryInfo.ToString());
            }
            StopProgress();
            ApplicationService.Invoke(() => { ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null); });
        }

		bool GetState(XBinaryBase binaryBase, XDevice gkParent)
		{
			var no = binaryBase.GetDatabaseNo(DatabaseType.Gk);
			var sendResult = SendManager.Send(gkParent, 2, 12, 68, BytesHelper.ShortToBytes(no));
			if (sendResult.HasError)
			{
				ConnectionChanged(false);
				return false;
			}
			if (sendResult.Bytes.Count != 68)
			{
				ApplicationService.Invoke(() => { SetDBMissmatch(binaryBase); });
				ConnectionChanged(false);
				return false;
			}
			ConnectionChanged(true);
			var binaryObjectState = new BinaryObjectState(sendResult.Bytes);
			CheckDBMissmatch(binaryBase, binaryObjectState);
			ApplicationService.Invoke(() => { SetObjectStates(binaryBase, binaryObjectState.States); });
			return true;
		}

		void SetObjectStates(XBinaryBase binaryBase, List<XStateType> states)
		{
			binaryBase.GetXBaseState().States = states;
		}

		void SetDBMissmatch(XBinaryBase binaryBase)
		{
			binaryBase.GetXBaseState().IsMissmatch = true;
		}

		void CheckDBMissmatch(XBinaryBase binaryBase, BinaryObjectState binaryObjectState)
		{
			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				if (device.Driver.DriverTypeNo != binaryObjectState.TypeNo)
					SetDBMissmatch(binaryBase);
				if (device.Driver.HasAddress && device.Driver.DriverType != XDriverType.GK && device.Driver.DriverType != XDriverType.KAU
					&& device.IntAddress != binaryObjectState.PhysicalAddress)
					SetDBMissmatch(binaryBase);
				if (device.GetNearestDatabaseNo() != binaryObjectState.AddressOncontroller)
					SetDBMissmatch(binaryBase);
			}
			if (binaryBase is XZone)
			{
				var zone = binaryBase as XZone;
				if (binaryObjectState.TypeNo != 0x100)
					SetDBMissmatch(binaryBase);
			}
			if (binaryBase is XDirection)
			{
				var direction = binaryBase as XDirection;
				if (binaryObjectState.TypeNo != 0x106)
					SetDBMissmatch(binaryBase);
			}
		}

		void CheckServiceRequired(XBinaryBase binaryBase, JournalItem journalItem)
		{
			if(journalItem.Name != "Запыленность")
				return;

			if (binaryBase is XDevice)
			{
				var device = binaryBase as XDevice;
				bool isDusted = journalItem.YesNo;
				ApplicationService.Invoke(() => { device.DeviceState.IsService = isDusted; });
			}
		}
	}
}
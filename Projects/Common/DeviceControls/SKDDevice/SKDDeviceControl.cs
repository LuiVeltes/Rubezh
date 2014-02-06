﻿using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using Infrustructure.Plans.Devices;
using XFiresecAPI;

namespace DeviceControls.SKDDevice
{
	public class SKDDeviceControl : BaseDeviceControl<SKDLibraryFrame, XStateClass>
	{
		public XStateClass StateClass { get; set; }

		protected override IEnumerable<ILibraryState<SKDLibraryFrame, XStateClass>> GetStates()
		{
			var libraryDevice = SKDManager.SKDLibraryConfiguration.Devices.FirstOrDefault(x => x.DriverId == DriverId);
			if (libraryDevice == null)
			{
				Logger.Error("SKDDeviceControl.Update libraryDevice = null " + DriverId.ToString());
				return null;
			}

			var libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateClass == StateClass);
			if (libraryState == null)
			{
				libraryState = libraryDevice.States.FirstOrDefault(x => x.Code == null && x.StateClass == XStateClass.No);
				if (libraryState == null)
				{
					Logger.Error("SKDDeviceControl.Update libraryState = null " + DriverId.ToString());
					return null;
				}
			}

			var resultLibraryStates = new List<ILibraryState<SKDLibraryFrame, XStateClass>>();
			if (libraryState != null)
				resultLibraryStates.Add(libraryState);
			return resultLibraryStates;
		}
	}
}
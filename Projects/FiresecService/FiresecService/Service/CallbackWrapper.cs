﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecService.Service
{
	public class CallbackWrapper
	{
		FiresecService FiresecService;

		public CallbackWrapper(FiresecService firesecService)
		{
			FiresecService = firesecService;
		}

		public void OnNewJournalRecord(JournalRecord journalRecord)
		{
			SafeCall((x) => { x.FiresecCallbackService.NewJournalRecord(journalRecord); });
		}

		public void OnDeviceStatesChanged(List<DeviceState> deviceStates)
		{
			SafeCall((x) => { x.Callback.DeviceStateChanged(deviceStates); });
		}

		public void OnDeviceParametersChanged(List<DeviceState> deviceParameters)
		{
			SafeCall((x) => { x.Callback.DeviceParametersChanged(deviceParameters); });
		}

		public void OnZoneStateChanged(ZoneState zoneState)
		{
			SafeCall((x) => { x.Callback.ZoneStateChanged(zoneState); });
		}

		public bool OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			try
			{
				var result = FiresecService.FiresecCallbackService.Progress(stage, comment, percentComplete, bytesRW);
				return result;
			}
			catch (ObjectDisposedException)
			{
				FiresecService.DisposeComServer();
			}
			catch (Exception)
			{
				FiresecService.DisposeComServer();
			}
			return true;
		}

		void SafeCall(Action<FiresecService> action)
		{
			if (FiresecService.IsSubscribed)
				try
				{
					action(FiresecService);
				}
				catch (ObjectDisposedException)
				{
					FiresecService.DisposeComServer();
				}
				catch (Exception)
				{
					FiresecService.DisposeComServer();
				}
		}
	}
}
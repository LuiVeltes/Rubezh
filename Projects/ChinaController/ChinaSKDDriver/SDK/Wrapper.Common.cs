﻿using System;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using FiresecAPI.SKD;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		#region Helpers
		public static DateTime NET_TIMEToDateTime(NativeWrapper.NET_TIME netTime)
		{
			DateTime dateTime = DateTime.MinValue;
			try
			{
				if (netTime.dwYear <= 0 || netTime.dwMonth <= 0 || netTime.dwDay <= 0)
					return new DateTime();
				dateTime = new DateTime(netTime.dwYear, netTime.dwMonth, netTime.dwDay, netTime.dwHour, netTime.dwMinute, netTime.dwSecond);
			}
			catch { }
			return dateTime;
		}
		#endregion

		#region Common
		public int LoginID { get; private set; }

		public int Connect(string ipAddress, int port, string login, string password)
		{
			LoginID = NativeWrapper.WRAP_Connect(ipAddress, port, login, password);
			return LoginID;
		}

		public bool Disconnect()
		{
			var result = NativeWrapper.WRAP_Disconnect(LoginID);
			return result;
		}
		#endregion

		#region CommonDevice
		public DeviceSoftwareInfo GetDeviceSoftwareInfo()
		{
			NativeWrapper.WRAP_DevConfig_TypeAndSoftInfo_Result outResult;
			var result = NativeWrapper.WRAP_GetSoftwareInfo(LoginID, out outResult);

			DeviceSoftwareInfo deviceSoftwareInfo = null;
			if (result)
			{
				deviceSoftwareInfo = new DeviceSoftwareInfo();
				deviceSoftwareInfo.DeviceType = outResult.szDevType;
				deviceSoftwareInfo.SoftwareVersion = outResult.szSoftWareVersion;
				try
				{
					if (outResult.dwSoftwareBuildDate_Year > 0 && outResult.dwSoftwareBuildDate_Month > 0 && outResult.dwSoftwareBuildDate_Day > 0)
						deviceSoftwareInfo.SoftwareBuildDate = new DateTime(outResult.dwSoftwareBuildDate_Year, outResult.dwSoftwareBuildDate_Month, outResult.dwSoftwareBuildDate_Day);
				}
				catch { }
			}
			return deviceSoftwareInfo;
		}

		public DeviceNetInfo GetDeviceNetInfo()
		{
			NativeWrapper.WRAP_CFG_NETWORK_INFO_Result outResult;
			var result = NativeWrapper.WRAP_Get_NetInfo(LoginID, out outResult);

			DeviceNetInfo deviceNetInfo = null;
			if (result)
			{
				deviceNetInfo = new DeviceNetInfo();
				deviceNetInfo.IP = outResult.szIP;
				deviceNetInfo.SubnetMask = outResult.szSubnetMask;
				deviceNetInfo.DefaultGateway = outResult.szDefGateway;
				deviceNetInfo.MTU = outResult.nMTU;
			}
			return deviceNetInfo;
		}

		public bool SetDeviceNetInfo(DeviceNetInfo deviceNetInfo)
		{
			var result = NativeWrapper.WRAP_Set_NetInfo(LoginID, deviceNetInfo.IP, deviceNetInfo.SubnetMask, deviceNetInfo.DefaultGateway, deviceNetInfo.MTU);
			return result;
		}

		public string GetDeviceMacAddress()
		{
			NativeWrapper.WRAP_DevConfig_MAC_Result outResult;
			var result = NativeWrapper.WRAP_GetMacAddress(LoginID, out outResult);
			if (result)
			{
				var macAddress = outResult.szMAC;
				return macAddress;
			}
			return null;
		}

		public int GetMaxPageSize()
		{
			NativeWrapper.WRAP_DevConfig_RecordFinderCaps_Result outResult;
			var result = NativeWrapper.WRAP_GetMaxPageSize(LoginID, out outResult);
			if (result)
			{
				var maxPageSize = outResult.nMaxPageSize;
				return maxPageSize;
			}
			return -1;
		}

		public DateTime GetDateTime()
		{
			NativeWrapper.NET_TIME outResult;
			var result = NativeWrapper.WRAP_GetCurrentTime(LoginID, out outResult);
			if (result)
			{
				try
				{
					var dateTime = new DateTime(outResult.dwYear, outResult.dwMonth, outResult.dwDay, outResult.dwHour, outResult.dwMinute, outResult.dwSecond);
					return dateTime;
				}
				catch { }
			}
			return DateTime.MinValue;
		}

		public bool SetDateTime(DateTime dateTime)
		{
			var result = NativeWrapper.WRAP_SetCurrentTime(LoginID, dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
			return result;
		}

		public bool Reset()
		{
			var result = NativeWrapper.WRAP_DeleteCfgFile(LoginID);
			return result;
		}

		public bool Reboot()
		{
			var result = NativeWrapper.WRAP_ReBoot(LoginID);
			return result;
		}

		public string GetProjectPassword()
		{
			NativeWrapper.WRAP_GeneralConfig_Password outResult;
			var result = NativeWrapper.WRAP_GetProjectPassword(LoginID, out outResult);
			if (result)
			{
				var projectPassword = outResult.szProjectPassword;
				return projectPassword;
			}
			else
			{
				return null;
			}
		}

		public bool SetProjectPassword(string projectPassword)
		{
			var result = NativeWrapper.WRAP_SetProjectPassword(LoginID, projectPassword);
			return result;
		}

		public int GetLogsCount()
		{
			NativeWrapper.QUERY_DEVICE_LOG_PARAM logParam = new NativeWrapper.QUERY_DEVICE_LOG_PARAM();
			var result = NativeWrapper.WRAP_GetLogCount(LoginID, ref logParam);
			return result;
		}

		public DoorConfiguration GetDoorConfiguration(int doorNo)
		{
			int structSize = Marshal.SizeOf(typeof(NativeWrapper.CFG_ACCESS_EVENT_INFO));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_GetDoorConfiguration(LoginID, doorNo, intPtr);

			NativeWrapper.CFG_ACCESS_EVENT_INFO outResult = (NativeWrapper.CFG_ACCESS_EVENT_INFO)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.CFG_ACCESS_EVENT_INFO)));

			Marshal.FreeHGlobal(intPtr);
			intPtr = IntPtr.Zero;

			if (result)
			{
				var doorConfiguration = new DoorConfiguration();
				doorConfiguration.ChannelName = outResult.szChannelName;
				doorConfiguration.AccessState = (AccessState)outResult.emState;
				doorConfiguration.AccessMode = (AccessMode)outResult.emMode;
				doorConfiguration.EnableMode = outResult.nEnableMode;
				doorConfiguration.DoorOpenMethod = (DoorOpenMethod)outResult.emDoorOpenMethod;
				doorConfiguration.UnlockHoldInterval = outResult.nUnlockHoldInterval;
				doorConfiguration.CloseTimeout = outResult.nCloseTimeout;
				doorConfiguration.OpenAlwaysTimeIndex = outResult.nOpenAlwaysTimeIndex;
				doorConfiguration.HolidayTimeRecoNo = outResult.nHolidayTimeRecoNo;
				doorConfiguration.IsBreakInAlarmEnable = outResult.bBreakInAlarmEnable;
				doorConfiguration.IsRepeatEnterAlarmEnable = outResult.bRepeatEnterAlarm;
				doorConfiguration.IsDoorNotClosedAlarmEnable = outResult.bDoorNotClosedAlarmEnable;
				doorConfiguration.IsDuressAlarmEnable = outResult.bDuressAlarmEnable;
				doorConfiguration.IsSensorEnable = outResult.bSensorEnable;

				var doorDayIntervalsCollection = new DoorDayIntervalsCollection();
				for (int i = 0; i < 7; i++)
				{
					var doorDayInterval = new DoorDayInterval();
					for (int j = 0; j < 4; j++)
					{
						var cfg_DOOROPEN_TIMESECTION_INFO = outResult.stuDoorTimeSection[i * 4 + j];
						var doorDayIntervalPart = new DoorDayIntervalPart();
						doorDayIntervalPart.StartHour = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuStartTime.dwHour;
						doorDayIntervalPart.StartMinute = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuStartTime.dwMinute;
						doorDayIntervalPart.EndHour = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuEndTime.dwHour;
						doorDayIntervalPart.EndMinute = cfg_DOOROPEN_TIMESECTION_INFO.stuTime.stuEndTime.dwMinute;
						doorDayIntervalPart.DoorOpenMethod = (SKDDoorConfiguration_DoorOpenMethod)cfg_DOOROPEN_TIMESECTION_INFO.emDoorOpenMethod;
						doorDayInterval.DoorDayIntervalParts.Add(doorDayIntervalPart);
					}
					doorDayIntervalsCollection.DoorDayIntervals.Add(doorDayInterval);
				}
				doorConfiguration.DoorDayIntervalsCollection = doorDayIntervalsCollection;
				return doorConfiguration;
			}
			return null;
		}

		public bool SetDoorConfiguration(DoorConfiguration doorConfiguration, int doorNo)
		{
			NativeWrapper.CFG_ACCESS_EVENT_INFO info = new NativeWrapper.CFG_ACCESS_EVENT_INFO();
			info.szChannelName = doorConfiguration.ChannelName;
			info.emState = (NativeWrapper.CFG_ACCESS_STATE)doorConfiguration.AccessState;
			info.emMode = (NativeWrapper.CFG_ACCESS_MODE)doorConfiguration.AccessMode;
			info.nEnableMode = doorConfiguration.EnableMode;
			info.bSnapshotEnable = doorConfiguration.IsSnapshotEnable ? (byte)0 : (byte)1;
			info.abDoorOpenMethod = doorConfiguration.UseDoorOpenMethod ? (byte)0 : (byte)1;
			info.abUnlockHoldInterval = doorConfiguration.UseUnlockHoldInterval ? (byte)0 : (byte)1;
			info.abCloseTimeout = doorConfiguration.UseCloseTimeout ? (byte)0 : (byte)1;
			info.abOpenAlwaysTimeIndex = doorConfiguration.UseOpenAlwaysTimeIndex ? (byte)0 : (byte)1;
			info.abHolidayTimeIndex = doorConfiguration.UseHolidayTimeIndex ? (byte)0 : (byte)1;
			info.abBreakInAlarmEnable = doorConfiguration.UseBreakInAlarmEnable ? (byte)0 : (byte)1;
			info.abRepeatEnterAlarmEnable = doorConfiguration.UseRepeatEnterAlarmEnable ? (byte)0 : (byte)1;
			info.abDoorNotClosedAlarmEnable = doorConfiguration.UseDoorNotClosedAlarmEnable ? (byte)0 : (byte)1;
			info.abDuressAlarmEnable = doorConfiguration.UseDuressAlarmEnable ? (byte)0 : (byte)1;
			info.abDoorTimeSection = doorConfiguration.UseDoorTimeSection ? (byte)0 : (byte)1;
			info.abSensorEnable = doorConfiguration.UseSensorEnable ? (byte)0 : (byte)1;
			info.emDoorOpenMethod = (NativeWrapper.CFG_DOOR_OPEN_METHOD)doorConfiguration.DoorOpenMethod;
			info.nUnlockHoldInterval = doorConfiguration.UnlockHoldInterval;
			info.nCloseTimeout = doorConfiguration.CloseTimeout;
			info.nOpenAlwaysTimeIndex = doorConfiguration.OpenAlwaysTimeIndex;
			info.nHolidayTimeRecoNo = doorConfiguration.HolidayTimeRecoNo;
			info.bBreakInAlarmEnable = doorConfiguration.IsBreakInAlarmEnable;
			info.bRepeatEnterAlarm = doorConfiguration.IsRepeatEnterAlarmEnable;
			info.bDoorNotClosedAlarmEnable = doorConfiguration.IsDoorNotClosedAlarmEnable;
			info.bDuressAlarmEnable = doorConfiguration.IsDuressAlarmEnable;
			info.bSensorEnable = doorConfiguration.IsSensorEnable;

			info.stuDoorTimeSection = new NativeWrapper.CFG_DOOROPEN_TIMESECTION_INFO[7 * 4];
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					var doorDayIntervalPart = doorConfiguration.DoorDayIntervalsCollection.DoorDayIntervals[i].DoorDayIntervalParts[j];
					info.stuDoorTimeSection[i].stuTime.stuStartTime.dwHour = doorDayIntervalPart.StartHour;
					info.stuDoorTimeSection[i].stuTime.stuStartTime.dwMinute = doorDayIntervalPart.StartMinute;
					info.stuDoorTimeSection[i].stuTime.stuStartTime.dwSecond = 0;
					info.stuDoorTimeSection[i].stuTime.stuEndTime.dwHour = doorDayIntervalPart.EndHour;
					info.stuDoorTimeSection[i].stuTime.stuEndTime.dwMinute = doorDayIntervalPart.EndMinute;
					info.stuDoorTimeSection[i].stuTime.stuEndTime.dwSecond = 0;
					info.stuDoorTimeSection[i].emDoorOpenMethod = (NativeWrapper.CFG_DOOR_OPEN_METHOD)doorDayIntervalPart.DoorOpenMethod;
				}
			}

			var result = NativeWrapper.WRAP_SetDoorConfiguration(LoginID, doorNo, ref info);
			return result;
		}

		#endregion
	}
}
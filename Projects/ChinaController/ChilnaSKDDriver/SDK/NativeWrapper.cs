﻿using System;
using System.Runtime.InteropServices;

namespace ChinaSKDDriverNativeApi
{
	public class NativeWrapper
	{
		#region Common
		[DllImport(@"CPPWrapper.dll")]
		public static extern void WRAP_Initialize();

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Connect(string ipAddress, int port, string userName, string password);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Disconnect(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Reconnect(string ipAddress, int port, string userName, string password);

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_TIME
		{
			public Int32 dwYear;
			public Int32 dwMonth;
			public Int32 dwDay;
			public Int32 dwHour;
			public Int32 dwMinute;
			public Int32 dwSecond;
		}

		public enum DH_LOG_QUERY_TYPE
		{
			DHLOG_ALL = 0,
			DHLOG_SYSTEM,
			DHLOG_CONFIG,
			DHLOG_STORAGE,
			DHLOG_ALARM,
			DHLOG_RECORD,
			DHLOG_ACCOUNT,
			DHLOG_CLEAR,
			DHLOG_PLAYBACK,
			DHLOG_MANAGER
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct QUERY_DEVICE_LOG_PARAM
		{
			public DH_LOG_QUERY_TYPE emLogType;
			public NET_TIME stuStartTime;
			public NET_TIME stuEndTime;
			public Int32 nStartNum;
			public Int32 nEndNum;
			public Byte nLogStuType;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public Byte[] reserved;
			public UInt32 nChannelID;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
			public Byte[] bReserved;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct DHDEVTIME
		{
			public Int32 second;
			public Int32 minute;
			public Int32 hour;
			public Int32 day;
			public Int32 month;
			public Int32 year;
		}

		public enum CFG_ACCESS_STATE
		{
			ACCESS_STATE_NORMAL,
			ACCESS_STATE_CLOSEALWAYS,
			ACCESS_STATE_OPENALWAYS,
		}

		public enum CFG_ACCESS_MODE
		{
			ACCESS_MODE_HANDPROTECTED,
			ACCESS_MODE_SAFEROOM,
			ACCESS_MODE_OTHER,
		}

		public enum CFG_DOOR_OPEN_METHOD
		{
			CFG_DOOR_OPEN_METHOD_UNKNOWN = 0,
			CFG_DOOR_OPEN_METHOD_PWD_ONLY,
			CFG_DOOR_OPEN_METHOD_CARD,
			CFG_DOOR_OPEN_METHOD_PWD_OR_CARD,
			CFG_DOOR_OPEN_METHOD_CARD_FIRST,
			CFG_DOOR_OPEN_METHOD_PWD_FIRST,
			CFG_DOOR_OPEN_METHOD_SECTION,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_TIME
		{
			public Int32 dwHour;
			public Int32 dwMinute;
			public Int32 dwSecond;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_TIME_PERIOD
		{
			public CFG_TIME stuStartTime;
			public CFG_TIME stuEndTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_DOOROPEN_TIMESECTION_INFO
		{
			public CFG_TIME_PERIOD stuTime;
			public CFG_DOOR_OPEN_METHOD emDoorOpenMethod;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_DevConfig_TypeAndSoftInfo_Result
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szDevType;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public char[] szSoftWareVersion;
			public Int32 dwSoftwareBuildDate_Year;
			public Int32 dwSoftwareBuildDate_Month;
			public Int32 dwSoftwareBuildDate_Day;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetSoftwareInfo(Int32 loginID, out WRAP_DevConfig_TypeAndSoftInfo_Result result);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_CFG_NETWORK_INFO_Result
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public char[] szIP;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public char[] szSubnetMask;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
			public char[] szDefGateway;
			public Int32 nMTU;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_NetInfo(int loginID, out WRAP_CFG_NETWORK_INFO_Result stuNetwork);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Set_NetInfo(int loginID, string ip, string mask, string gate, int mtu);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_DevConfig_MAC_Result
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
			public char[] szMAC;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetMacAddress(int loginID, out WRAP_DevConfig_MAC_Result result);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_DevConfig_RecordFinderCaps_Result
		{
			public int nMaxPageSize;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetMaxPageSize(int loginID, out WRAP_DevConfig_RecordFinderCaps_Result result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetCurrentTime(int loginID, out NET_TIME result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetCurrentTime(int loginID, int dwYear, int dwMonth, int dwDay, int dwHour, int dwMinute, int dwSecond);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_LogItem
		{
			public int nLogType;
			public DHDEVTIME stuOperateTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public char[] szOperator;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szOperation;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4 * 1024)]
			public char[] szDetailContext;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_Dev_QueryLogList_Result
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public WRAP_LogItem[] Logs;
			public int Test;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_QueryLogList(int loginID, out WRAP_Dev_QueryLogList_Result result);

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_GeneralConfig_Password
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szProjectPassword;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetProjectPassword(int loginID, out WRAP_GeneralConfig_Password result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetProjectPassword(int loginID, string password);

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_ACCESS_EVENT_INFO
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public char[] szChannelName;
			public CFG_ACCESS_STATE emState;
			public CFG_ACCESS_MODE emMode;
			public int nEnableMode;
			public byte bSnapshotEnable;

			public byte abDoorOpenMethod;
			public byte abUnlockHoldInterval;
			public byte abCloseTimeout;
			public byte abOpenAlwaysTimeIndex;
			public byte abHolidayTimeIndex;
			public byte abBreakInAlarmEnable;
			public byte abRepeatEnterAlarmEnable;
			public byte abDoorNotClosedAlarmEnable;
			public byte abDuressAlarmEnable;
			public byte abDoorTimeSection;
			public byte abSensorEnable;
			public byte byReserved;

			public CFG_DOOR_OPEN_METHOD emDoorOpenMethod;
			public int nUnlockHoldInterval;
			public int nCloseTimeout;
			public int nOpenAlwaysTimeIndex;
			public int nHolidayTimeRecoNo;
			public bool bBreakInAlarmEnable;
			public bool bRepeatEnterAlarm;
			public bool bDoorNotClosedAlarmEnable;
			public bool bDuressAlarmEnable;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 4)]
			public CFG_DOOROPEN_TIMESECTION_INFO[] stuDoorTimeSection;
			public bool bSensorEnable;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_ACCESS_EVENT_INFO_Bools
		{
			public bool bSnapshotEnable;
			public bool abDoorOpenMethod;
			public bool abUnlockHoldInterval;
			public bool abCloseTimeout;
			public bool abOpenAlwaysTimeIndex;
			public bool abHolidayTimeIndex;
			public bool abBreakInAlarmEnable;
			public bool abRepeatEnterAlarmEnable;
			public bool abDoorNotClosedAlarmEnable;
			public bool abDuressAlarmEnable;
			public bool abDoorTimeSection;
			public bool abSensorEnable;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetDoorConfiguration(int loginID, int channelNo, IntPtr result, IntPtr result2);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetDoorConfiguration(int loginID, int channelNo, ref CFG_ACCESS_EVENT_INFO accessEventInfo, ref CFG_ACCESS_EVENT_INFO_Bools boolsConfig);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_ReBoot(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_DeleteCfgFile(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetLogCount(int loginID, ref QUERY_DEVICE_LOG_PARAM logParam);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_OpenDoor(int loginID, int channelNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_CloseDoor(int loginID, int channelNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetDoorStatus(int loginID, int channelNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool TestStruct(IntPtr result);
		#endregion

		#region Cards
		public enum NET_ACCESSCTLCARD_STATE
		{
			NET_ACCESSCTLCARD_STATE_UNKNOWN = -1,
			NET_ACCESSCTLCARD_STATE_NORMAL = 0,
			NET_ACCESSCTLCARD_STATE_LOSE = 0x01,
			NET_ACCESSCTLCARD_STATE_LOGOFF = 0x02,
			NET_ACCESSCTLCARD_STATE_FREEZE = 0x04,
		}

		public enum NET_ACCESSCTLCARD_TYPE
		{
			NET_ACCESSCTLCARD_TYPE_UNKNOWN = -1,
			NET_ACCESSCTLCARD_TYPE_GENERAL,
			NET_ACCESSCTLCARD_TYPE_VIP,
			NET_ACCESSCTLCARD_TYPE_GUEST,
			NET_ACCESSCTLCARD_TYPE_PATROL,
			NET_ACCESSCTLCARD_TYPE_BLACKLIST,
			NET_ACCESSCTLCARD_TYPE_CORCE,
			NET_ACCESSCTLCARD_TYPE_MOTHERCARD = 0xff,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_CARD
		{
			public Int32 dwSize;
			public Int32 nRecNo;
			public NET_TIME stuCreateTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public Char[] szCardNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public Char[] szUserID;
			public NET_ACCESSCTLCARD_STATE emStatus;
			public NET_ACCESSCTLCARD_TYPE emType;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public Char[] szPsw;
			public Int32 nDoorNum;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public Int32[] sznDoors;
			public Int32 nTimeSectionNum;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public Int32[] sznTimeSectionNo;
			public Int32 nUserTime;
			public NET_TIME stuValidStartTime;
			public NET_TIME stuValidEndTime;
			public bool bIsValid;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Insert_Card(int loginID, ref NET_RECORDSET_ACCESS_CTL_CARD nativeCard);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Update_Card(int loginID, ref NET_RECORDSET_ACCESS_CTL_CARD nativeCard);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Remove_Card(int loginID, int recordNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_RemoveAll_Cards(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_Card_Info(int loginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_CARD

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Get_Cards_Count(int loginID);

		[StructLayout(LayoutKind.Sequential)]
		public struct CardsCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public NET_RECORDSET_ACCESS_CTL_CARD[] Cards;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetAll_Cards(int loginID, IntPtr result);
		#endregion

		#region CardRecs
		public enum NET_ACCESS_DOOROPEN_METHOD
		{
			NET_ACCESS_DOOROPEN_METHOD_UNKNOWN = 0,
			NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,
			NET_ACCESS_DOOROPEN_METHOD_CARD,
			NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,
			NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,
			NET_ACCESS_DOOROPEN_METHOD_REMOTE,
			NET_ACCESS_DOOROPEN_METHOD_BUTTON,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_CARDREC
		{
			public int dwSize;
			public int nRecNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szCardNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szPwd;
			public NET_TIME stuTime;
			public bool bStatus;
			public NET_ACCESS_DOOROPEN_METHOD emMethod;
			public int nDoor;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Insert_CardRec(int loginID, ref NET_RECORDSET_ACCESS_CTL_CARDREC nativeCardRec);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Update_CardRec(int loginID, ref NET_RECORDSET_ACCESS_CTL_CARDREC nativeCardRec);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Remove_CardRec(int loginID, int recordNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_RemoveAll_CardRecs(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_CardRec_Info(int loginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_CARDREC

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Get_CardRecs_Count(int loginID);

		[StructLayout(LayoutKind.Sequential)]
		public struct CardRecsCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public NET_RECORDSET_ACCESS_CTL_CARDREC[] CardRecs;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetAll_CardRecs(int loginID, IntPtr result);
		#endregion

		#region Passwords
		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_ACCESS_CTL_PWD
		{
			public int dwSize;
			public int nRecNo;
			public NET_TIME stuCreateTime;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szUserID;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szDoorOpenPwd;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szAlarmPwd;
			public int nDoorNum;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public int[] sznDoors;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Insert_Password(int loginID, ref NET_RECORDSET_ACCESS_CTL_PWD nativePassword);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Update_Password(int loginID, ref NET_RECORDSET_ACCESS_CTL_PWD nativePassword);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Remove_Password(int loginID, int recordNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_RemoveAll_Passwords(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_Password_Info(int loginID, int recordNo, IntPtr result); // NET_RECORDSET_ACCESS_CTL_PWD

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Get_Passwords_Count(int loginID);

		[StructLayout(LayoutKind.Sequential)]
		public struct PasswordsCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public NET_RECORDSET_ACCESS_CTL_PWD[] Passwords;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetAll_Passwords(int loginID, IntPtr result);
		#endregion

		#region Holidays
		[StructLayout(LayoutKind.Sequential)]
		public struct NET_RECORDSET_HOLIDAY
		{
			public int dwSize;
			public int nRecNo;
			public int nDoorNum;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public int[] sznDoors;
			public NET_TIME stuStartTime;
			public NET_TIME stuEndTime;
			public bool bEnable;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Insert_Holiday(int loginID, ref NET_RECORDSET_HOLIDAY nativeHoliday);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Update_Holiday(int loginID, ref NET_RECORDSET_HOLIDAY nativeHoliday);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Remove_Holiday(int loginID, int recordNo);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_RemoveAll_Holidays(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_Get_Holiday_Info(int loginID, int recordNo, IntPtr result); // NET_RECORDSET_HOLIDAY

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_Get_Holidays_Count(int loginID);

		[StructLayout(LayoutKind.Sequential)]
		public struct HolidaysCollection
		{
			public int Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public NET_RECORDSET_HOLIDAY[] Holidays;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetAll_Holidays(int loginID, IntPtr result);
		#endregion

		#region TimeShedules
		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_TIME_SECTION
		{
			public Int32 dwRecordMask;
			public Int32 nBeginHour;
			public Int32 nBeginMin;
			public Int32 nBeginSec;
			public Int32 nEndHour;
			public Int32 nEndMin;
			public Int32 nEndSec;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CFG_ACCESS_TIMESCHEDULE_INFO
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7 * 4)]
			public CFG_TIME_SECTION[] stuTime;
			public bool bEnable;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetTimeSchedule(int loginID, int index, out CFG_ACCESS_TIMESCHEDULE_INFO result);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_SetTimeSchedule(int loginID, int index, ref CFG_ACCESS_TIMESCHEDULE_INFO timeSheduleInfo);
		#endregion

		#region Events
		public enum NET_ACCESS_CTL_EVENT_TYPE
		{
			NET_ACCESS_CTL_EVENT_UNKNOWN = 0,
			NET_ACCESS_CTL_EVENT_ENTRY,
			NET_ACCESS_CTL_EVENT_EXIT,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WRAP_JournalItem
		{
			public int ExtraEventType;
			public int EventType;
			public NET_TIME DeviceDateTime;
			public int nDoor;
			public NET_ACCESS_CTL_EVENT_TYPE emEventType;
			public bool bStatus;
			public NET_ACCESSCTLCARD_TYPE emCardType;
			public NET_ACCESS_DOOROPEN_METHOD emOpenMethod;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public char[] szCardNo;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public char[] szPwd;
			public int nAction;
		}

		[DllImport(@"CPPWrapper.dll")]
		public static extern int WRAP_GetLastIndex(int loginID);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_GetJournalItem(int loginID, int index, out WRAP_JournalItem journalItem);

		[DllImport(@"CPPWrapper.dll")]
		public static extern bool WRAP_IsConnected(int loginID);
		#endregion
	}
}
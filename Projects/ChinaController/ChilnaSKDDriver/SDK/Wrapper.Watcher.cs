﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using System.Threading;
using System.Diagnostics;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		Thread WatcherThread;
		bool IsStopping;

		public void StartWatcher()
		{
			IsStopping = false;
			WatcherThread = new Thread(RunMonitoring);
			WatcherThread.Start();
		}

		public void StopWatcher()
		{
			IsStopping = true;
			if (WatcherThread != null)
			{
				WatcherThread.Join(TimeSpan.FromSeconds(2));
			}
		}

		void RunMonitoring()
		{
			var lastIndex = -1;
			while (true)
			{
				if (IsStopping)
					return;
				Thread.Sleep(100);

				try
				{
					var index = NativeWrapper.WRAP_GetLastIndex(LoginID);
					if (index > lastIndex)
					{
						for (int i = lastIndex + 1; i <= index; i++)
						{
							var wrapJournalItem = new NativeWrapper.WRAP_JournalItem();
							NativeWrapper.WRAP_GetJournalItem(LoginID, i, out wrapJournalItem);
							var journalItem = ParceJournal(wrapJournalItem);

							if (NewJournalItem != null)
								NewJournalItem(journalItem);
						}
						lastIndex = index;
					}
				}
				catch
				{
					//
				}
			}
		}

		const int DH_ALARM_ACCESS_CTL_EVENT = 0x3181;
		const int DH_ALARM_ACCESS_CTL_NOT_CLOSE = 0x3177;
		const int DH_ALARM_ACCESS_CTL_BREAK_IN = 0x3178;
		const int DH_ALARM_ACCESS_CTL_REPEAT_ENTER = 0x3179;
		const int DH_ALARM_ACCESS_CTL_DURESS = 0x3180;

		SKDJournalItem ParceJournal(NativeWrapper.WRAP_JournalItem wrapJournalItem)
		{
			var journalItem = new SKDJournalItem();
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = Wrapper.NET_TIMEToDateTime(wrapJournalItem.DeviceDateTime);

			switch(wrapJournalItem.ExtraEventType)
			{
				case 1:
					journalItem.EventNameType = FiresecAPI.Events.GlobalEventNameEnum.Потеря_связи;
					journalItem.DeviceDateTime = DateTime.Now;
					return journalItem;

				case 2:
					journalItem.EventNameType = FiresecAPI.Events.GlobalEventNameEnum.Восстановление_связи;
					journalItem.DeviceDateTime = DateTime.Now;
					return journalItem;
			}

			var description = "";
			switch(wrapJournalItem.EventType)
			{
				case DH_ALARM_ACCESS_CTL_EVENT:
					journalItem.EventNameType = FiresecAPI.Events.GlobalEventNameEnum.Проход;
					var doorNo = wrapJournalItem.nDoor;
					var eventType = wrapJournalItem.emEventType;
					var isStatus = wrapJournalItem.bStatus;
					var cardType = wrapJournalItem.emCardType;
					var doorOpenMethod = wrapJournalItem.emOpenMethod;
					var cardNo = CharArrayToString(wrapJournalItem.szCardNo);
					var password = CharArrayToString(wrapJournalItem.szPwd);
					description = eventType.ToString() + " " + isStatus.ToString() + " " + cardType.ToString() + " " + doorOpenMethod + " " + cardNo + " " + password;
					break;

				case DH_ALARM_ACCESS_CTL_NOT_CLOSE:
					journalItem.EventNameType = FiresecAPI.Events.GlobalEventNameEnum.Дверь_не_закрыта;
					doorNo = wrapJournalItem.nDoor;
					var action = wrapJournalItem.nAction;
					break;

				case DH_ALARM_ACCESS_CTL_BREAK_IN:
					journalItem.EventNameType = FiresecAPI.Events.GlobalEventNameEnum.Взлом;
					doorNo = wrapJournalItem.nDoor;
					break;

				case DH_ALARM_ACCESS_CTL_REPEAT_ENTER:
					journalItem.EventNameType = FiresecAPI.Events.GlobalEventNameEnum.Повторный_проход;
					doorNo = wrapJournalItem.nDoor;
					break;

				case DH_ALARM_ACCESS_CTL_DURESS:
					journalItem.EventNameType = FiresecAPI.Events.GlobalEventNameEnum.Принуждение;
					doorNo = wrapJournalItem.nDoor;
					cardNo = CharArrayToString(wrapJournalItem.szCardNo);
					break;

				default:
					journalItem.EventNameType = FiresecAPI.Events.GlobalEventNameEnum.Неизвестное_событие;
					break;
			}
			journalItem.Description = description;

			return journalItem;
		}
	}
}
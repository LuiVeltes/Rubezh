﻿using System;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public partial class Watcher
	{
		bool IsConnected = true;
		int ConnectionLostCount = 0;
		object connectionChangedLocker = new object();

		public void ConnectionChanged(bool isConnected)
		{
			lock (connectionChangedLocker)
			{
				if (!isConnected)
				{
					ConnectionLostCount++;
					if (ConnectionLostCount < 3)
						return;
				}
				else
					ConnectionLostCount = 0;

				if (IsConnected != isConnected)
				{
					var journalItem = new XJournalItem()
					{
						SystemDateTime = DateTime.Now,
						DeviceDateTime = DateTime.Now,
						GKIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice),
						JournalObjectType = XJournalObjectType.System,
						StateClass = XStateClass.Unknown,
						ObjectStateClass = XStateClass.Norm,
						JournalEventNameType = isConnected ? JournalEventNameType.Восстановление_связи_с_прибором : JournalEventNameType.Потеря_связи_с_прибором,
					};
					AddJournalItem(journalItem);

					IsConnected = isConnected;
					if (isConnected)
					{
						var hashBytes = GKFileInfo.CreateHash1(XManager.DeviceConfiguration, GkDatabase.RootDevice);
						var gkFileReaderWriter = new GKFileReaderWriter();
						var gkFileInfo = gkFileReaderWriter.ReadInfoBlock(GkDatabase.RootDevice);
						IsHashFailure = gkFileInfo == null || !GKFileInfo.CompareHashes(hashBytes, gkFileInfo.Hash1);
					}

					foreach (var descriptor in GkDatabase.Descriptors)
					{
						descriptor.XBase.InternalState.IsConnectionLost = !isConnected;
					}
					NotifyAllObjectsStateChanged();
				}
			}
		}
	}
}
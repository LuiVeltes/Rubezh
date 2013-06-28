﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;
using ServerFS2.Service;
using System.Diagnostics;

namespace ServerFS2.Monitoring
{
	public class MonitoringPanel
	{
		const int maxSequentUnAnswered = 10;
		const int maxMessages = 1024;
		const int maxSecMessages = 1024;
		public const int betweenDevicesSpan = 0;
		public const int betweenCyclesSpan = 1000;
		public const int requestExpiredTime = 5;
		public static readonly object Locker = new object();

		int SequentUnAnswered;
		int RealChildIndex;
		public int AnsweredCount { get; set; }
		public int UnAnsweredCount { get; set; }

		public Device PanelDevice { get; private set; }
		List<Device> RealChildren;
		DeviceStatesManager DeviceStatesManager;
		public bool IsConnectionLost { get; private set; }
		public bool IsInitialized { get; private set; }

		public static event Action<FS2JournalItem> NewJournalItem;
		public List<string> ResetStateIds { get; set; }
		public List<Device> DevicesToIgnore { get; set; }
		public List<Device> DevicesToResetIgnore { get; set; }
		public List<CommandItem> CommandItems { get; set; }
		public int LastDeviceIndex { get; set; }
		public bool IsReadingNeeded { get; set; }
		public bool IsStateRefreshNeeded { get; set; }
		public bool IsManipulationNeeded { get; set; }
		public List<Request> Requests { get; set; }
		public int FirstSystemIndex { get; set; }

		int _lastSystemIndex;
		public int LastSystemIndex
		{
			get { return _lastSystemIndex; }
			set
			{
				_lastSystemIndex = value;
				XmlJournalHelper.SetLastId(PanelDevice, value);
			}
		}

		public MonitoringPanel(Device device)
		{
			PanelDevice = device;
			Requests = new List<Request>();
			ResetStateIds = new List<string>();
			DevicesToIgnore = new List<Device>();
			CommandItems = new List<CommandItem>();
			LastSystemIndex = XmlJournalHelper.GetLastId(device);
			FirstSystemIndex = -1;
			SequentUnAnswered = 0;
			IsReadingNeeded = false;
			RealChildren = PanelDevice.GetRealChildren();
			DeviceStatesManager = new DeviceStatesManager();
		}

		public bool Initialize()
		{
			DeviceStatesManager.CanNotifyClients = false;
			IsInitialized = DeviceStatesManager.ReadConfigurationAndUpdateStates(PanelDevice);
			if (!IsInitialized)
			{
				IsConnectionLost = true;
				PanelDevice.DeviceState.IsPanelConnectionLost = true;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				return false;
			}
			else
			{
				IsConnectionLost = false;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
			}
			DeviceStatesManager.UpdatePanelState(PanelDevice);
			DeviceStatesManager.CanNotifyClients = true;
			return true;
		}

		public void CheckTasks()
		{
			if (IsInitialized)
			{
				if (IsReadingNeeded)
				{
					var journalItems = GetNewItems();
					DeviceStatesManager.UpdateDeviceStateOnJournal(journalItems);
					DeviceStatesManager.UpdatePanelState(PanelDevice);
					DeviceStatesManager.UpdatePanelParameters(PanelDevice);
				}
				if (ResetStateIds != null && ResetStateIds.Count > 0)
				{
					ServerHelper.ResetOnePanelStates(PanelDevice, ResetStateIds);
					ResetStateIds.Clear();
					DeviceStatesManager.UpdatePanelState(PanelDevice);
					OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Команда оператора. Сброс"));
				}

				if (DevicesToIgnore != null && DevicesToIgnore.Count > 0)
				{
					foreach (var deviceToIgnore in DevicesToIgnore)
					{
						USBManager.Send(PanelDevice, 0x02, 0x54, 0x0B, 0x01, 0x00, deviceToIgnore.AddressOnShleif, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
					}
					DevicesToIgnore = new List<Device>();
				}
				if (DevicesToResetIgnore != null && DevicesToResetIgnore.Count > 0)
				{
					foreach (var deviceToIgnore in DevicesToResetIgnore)
					{
						USBManager.Send(PanelDevice, 0x02, 0x54, 0x0B, 0x00, 0x00, deviceToIgnore.AddressOnShleif, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
					}
					DevicesToResetIgnore = new List<Device>();
				}
				if (CommandItems != null && CommandItems.Count > 0)
				{
					CommandItems.ForEach(x => x.Send());
					CommandItems = new List<CommandItem>();
				}
				if (IsStateRefreshNeeded)
				{
					DeviceStatesManager.UpdatePanelState(PanelDevice);
					foreach (var device in RealChildren)
					{
						DeviceStatesManager.UpdateDeviceStateAndParameters(device);
					}
					IsStateRefreshNeeded = false;
				}
				DeviceStatesManager.UpdatePanelExtraDevices(PanelDevice);

				DeviceStatesManager.UpdateDeviceStateAndParameters(RealChildren[RealChildIndex]);
				NextIndextoGetParams();
			}

			CheckConnectionLost();
			RequestLastIndex();
		}

		public void OnResponceRecieved(Request request, Response response)
		{
			AnsweredCount++;
			if (request.RequestType == RequestTypes.ReadIndex)
			{
				LastIndexReceived(response);
			}
			lock (Locker)
				Requests.RemoveAll(x => x != null && x.Id == request.Id);
		}

		void OnNewJournalItem(FS2JournalItem fsJournalItem)
		{
			CallbackManager.NewJournalItems(new List<FS2JournalItem>() { fsJournalItem } );
			DatabaseHelper.AddJournalItem(fsJournalItem);
			if (NewJournalItem != null)
				NewJournalItem(fsJournalItem);
		}

		void CheckConnectionLost()
		{
			var requestsToDelete = new List<Request>();
			lock (Locker)
				foreach (var request in Requests)
				{
					if (request != null && (DateTime.Now - request.StartTime).TotalSeconds >= requestExpiredTime)
					{
						requestsToDelete.Add(request);
						UnAnsweredCount++;
						SequentUnAnswered++;
					}
				}
			requestsToDelete.ForEach(x => Requests.Remove(x));
			if (SequentUnAnswered > maxSequentUnAnswered)
			{
				OnConnectionLost();
			}
		}

		public void OnConnectionLost()
		{
			if (!IsConnectionLost)
			{
				IsConnectionLost = true;
				PanelDevice.DeviceState.IsPanelConnectionLost = true;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				OnConnectionChanged();
				OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Потеря связи с прибором"));
			}
		}

		public void OnConnectionAppeared()
		{
			if (IsConnectionLost)
			{
				//var remoteSerialNo = ServerHelper.GetDeviceInformation(PanelDevice);
				//if (remoteSerialNo == null)
				//    return;
				//if (PanelDevice.Properties.FirstOrDefault(x => x.Name == "serialNo").Value == remoteSerialNo)
				//{
				//    OnWrongPanel();
				//    return;
				//}

				if (!IsInitialized)
				{
					Initialize();
					return;
				}

				IsConnectionLost = false;
				PanelDevice.DeviceState.IsPanelConnectionLost = false;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				OnConnectionChanged();
				OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Связь с прибором восстановлена"));
			}
		}

		public event Action ConnectionChanged;
		void OnConnectionChanged()
		{
			if (ConnectionChanged != null)
				ConnectionChanged();
		}

		void OnWrongPanel()
		{
			var deviceStatesManager = new DeviceStatesManager();
			var deviceStates = new List<DeviceState>();
			var panelState = PanelDevice.Driver.States.FirstOrDefault(y => y.Name == "Несоответствие версий БД с панелью");
			PanelDevice.DeviceState.IsWrongPanel = true;
			deviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
			foreach (var device in PanelDevice.GetRealChildren())
			{
				if (!device.DeviceState.ParentStates.Any(x => x.DriverState.Id == panelState.Id))
				{
					var parentDeviceState = new ParentDeviceState()
					{
						ParentDeviceUID = device.ParentPanel.UID,
						DriverState = panelState
					};
					device.DeviceState.ParentStates.Add(parentDeviceState);
				}

				device.DeviceState.IsWrongPanel = true;
				deviceStates.Add(device.DeviceState);
			}
			CallbackManager.DeviceStateChanged(deviceStates);
			OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Несоответствие версий БД с панелью"));
		}

		void NextIndextoGetParams()
		{
			RealChildIndex++;
			if (RealChildIndex + 1 >= RealChildren.Count)
				RealChildIndex = 0;
		}

		public void RequestLastIndex()
		{
			var request = new Request(RequestTypes.ReadIndex, new List<byte> { 0x01, 0x21, 0x00 });
			lock (Locker)
			{
				Requests.Add(request);
			}
			USBManager.SendAsync(PanelDevice, request);
			if (betweenDevicesSpan > 0)
				Thread.Sleep(betweenDevicesSpan);
		}

		public void LastIndexReceived(Response response)
		{
			if (response.HasError || response.Bytes.Count < 10)
			{
				return;
			}
			SequentUnAnswered = 0;
			OnConnectionAppeared();
			LastDeviceIndex = BytesHelper.ExtractInt(response.Bytes, 7);
			if (FirstSystemIndex == -1)
				FirstSystemIndex = LastDeviceIndex;
			if (LastSystemIndex == -1)
			{
				LastSystemIndex = LastDeviceIndex;
			}
			if (LastDeviceIndex - LastSystemIndex > maxMessages)
			{
				LastSystemIndex = LastDeviceIndex - maxMessages;
			}
			if (LastDeviceIndex > LastSystemIndex)
			{
				IsReadingNeeded = true;
			}
		}

		public List<FS2JournalItem> GetNewItems()
		{
			Requests.RemoveAll(x => x != null && x.RequestType == RequestTypes.ReadIndex);
			var journalItems = new List<FS2JournalItem>();
			for (int i = LastSystemIndex + 1; i <= LastDeviceIndex; i++)
			{
				var journalItem = JournalHelper.ReadItem(PanelDevice, i);
				if (journalItem != null)
				{
					OnNewJournalItem(journalItem);
					journalItems.Add(journalItem);
				}
			}
			LastSystemIndex = LastDeviceIndex;
			IsReadingNeeded = false;
			return journalItems;
		}

		public void SynchronizeTime()
		{
			var setDateTimeProperty = PanelDevice.Properties.FirstOrDefault(x => x.Name == "SetDateTime");
			if (setDateTimeProperty != null && setDateTimeProperty.Value == "1")
			{
				ServerHelper.SynchronizeTime(PanelDevice);
			}
		}			
	}
}
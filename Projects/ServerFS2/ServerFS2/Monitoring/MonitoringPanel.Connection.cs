﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;
using ServerFS2.Service;
using System.Diagnostics;
using ServerFS2.Operations;
using System.Text;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringPanel
	{
		int SequentUnAnswered = 0;
		int AnsweredCount;
		int UnAnsweredCount;
		string SerialNo;

		void CheckConnectionLost()
		{
			var requestsToDelete = new List<Request>();
			lock (Locker)
			{
				foreach (var request in Requests)
				{
					if (request != null && (DateTime.Now - request.StartTime).TotalSeconds >= RequestExpiredTime)
					{
						requestsToDelete.Add(request);
						UnAnsweredCount++;
						SequentUnAnswered++;
					}
				}
				requestsToDelete.ForEach(x => Requests.Remove(x));
			}
			if (SequentUnAnswered > MaxSequentUnAnswered)
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
				CheckWrongPanel();

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

		void CheckWrongPanel()
		{
			var serialNo = GetSerialNo();
			if (SerialNo == null)
			{
				SerialNo = serialNo;
			}
			else
			{
				if (serialNo != SerialNo)
				{
					if (!PanelDevice.DeviceState.IsDBMissmatch)
					{
						PanelDevice.DeviceState.IsDBMissmatch = true;
						DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
						OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Несоответствие версий БД с панелью"));
						return;
					}
				}
				else
				{
					if (PanelDevice.DeviceState.IsDBMissmatch)
					{
						PanelDevice.DeviceState.IsDBMissmatch = false;
						DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
						OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Несоответствие версий БД с панелью устранено"));
						return;
					}
				}
			}
		}

		string GetSerialNo()
		{
			var response = USBManager.Send(PanelDevice, 0x01, 0x52, 0x00, 0x00, 0x00, 0xF4, 0x0B);
			if (!response.HasError)
			{
				var result = new string(Encoding.Default.GetChars(response.Bytes.ToArray()));
				return result;
			}
			return null;
		}

		void CheckDBMissmatch()
		{

		}
	}
}
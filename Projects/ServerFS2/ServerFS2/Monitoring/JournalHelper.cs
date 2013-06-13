﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2;
using ServerFS2.ConfigurationWriter;

namespace ServerFS2.Monitoring
{
	public static class JournalHelper
	{
		static readonly object Locker = new object();

		public static int GetLastSecJournalItemId2Op(Device device)
		{
			try
			{
				var lastindex = SendByteCommandSync(new List<byte> { 0x21, 0x02 }, device);
				return BytesHelper.ExtractShort(lastindex, 9);
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetJournalCount(Device device)
		{
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
				return GetLastJournalItemId(device) + GetLastSecJournalItemId2Op(device);
			try
			{
				var firecount = SendByteCommandSync(new List<byte> { 0x24, 0x00 }, device);
				return BytesHelper.ExtractShort(firecount, 7);
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetFirstJournalItemId(Device device)
		{
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
				return 0;
			return GetLastJournalItemId(device) - GetJournalCount(device) + 1;
		}

		public static int GetLastJournalItemId(Device device)
		{
			try
			{
				var response = SendByteCommandSync(new List<byte> { 0x21, 0x00 }, device);
				var result = BytesHelper.ExtractInt(response, 7);
				return result;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message + "\n Проверьте связь с прибором");
				throw;
			}
		}

		public static List<FS2JournalItem> GetJournalItems(Device device, int lastindex, int firstindex)
		{
			var journalItems = new List<FS2JournalItem>();
			for (int i = firstindex; i <= lastindex; i++)
			{
				var journalItem = ReadItem(device, i);
				journalItems.Add(journalItem);
				MonitoringDevice.OnNewJournalItem(journalItem);
			}
			return journalItems;
		}

		public static List<FS2JournalItem> GetSecJournalItems2Op(Device device, int lastindex, int firstindex)
		{
			var journalItems = new List<FS2JournalItem>();
			for (int i = firstindex; i <= lastindex; i++)
				journalItems.Add(ReadSecItem(device, i));
			return journalItems;
		}

		public static List<FS2JournalItem> GetAllJournalItems(Device device)
		{
			return GetJournalItems(device, GetLastJournalItemId(device), GetFirstJournalItemId(device));
		}

		public static FS2JournalItem ReadItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			for (int j = 0; j < 15; j++)
			{
				var fsJournalItem = SendBytesAndParse(bytes, device);
				if (fsJournalItem != null)
					return fsJournalItem;
			}
			return null;
		}

		private static FS2JournalItem ReadSecItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x02 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			return SendBytesAndParse(bytes, device);
		}

		private static FS2JournalItem SendBytesAndParse(List<byte> commandBytes, Device device)
		{
			var bytes = new List<byte>();
			bytes.Add(GetMSChannelByte(device));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			var response = ServerHelper.SendCode(bytes);
			if (response == null)
				return null;
			lock (Locker)
			{
				var journalParser = new JournalParser();
				try
				{
					return journalParser.Parce(response);
				}
				catch
				{
					return null;
				}
			}
		}

		static List<byte> SendByteCommand(List<byte> commandBytes, Device device)
		{
			lock (Locker)
			{
				return ServerHelper.SendCodeToPanel(device, 0x01, commandBytes);
			}
		}

		static List<byte> SendByteCommandSync(List<byte> commandBytes, Device device)
		{
			return ServerHelper.SendCodeToPanel(device, 0x01, commandBytes);
		}

		public static void SendByteCommand(List<byte> commandBytes, Device device, int requestId)
		{
			var bytes = new List<byte>();
			bytes.Add(GetMSChannelByte(device));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			ServerHelper.SendCodeAsync(requestId, bytes);
		}

		private static byte GetMSChannelByte(Device device)
		{
			return 0x03;
		}
	}
}